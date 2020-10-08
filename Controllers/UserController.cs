using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure.Storage;
using WebPortfolioCoreApi.Models;
using WebPortfolioCoreApi.OtherModels;

namespace WebPortfolioCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // POST: api/user/checklogin
        // Check the correction of login credentials when signing in
        [HttpPost]
        [Route("checklogin")]
        public ActionResult CheckSignIn([FromBody] Users loginCredentials)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var user = (from u in context.Users
                            where u.Username == loginCredentials.Username
                            select u).FirstOrDefault();

                if (user != null)
                {
                    if (user.Password == loginCredentials.Password)
                    {
                        return Ok("Login succesfull!");
                    }
                    else
                    {
                        return NotFound("Inccorrect password!");
                    }
                }
                else
                {
                    return NotFound("Inccorrect username!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while checking the login credentials. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // POST: api/user/usernamecheck
        // Check the correction of login credentials when signing in
        [HttpPost]
        [Route("usernamecheck/{username}")]
        public ActionResult IsUsernameInUse(string username)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var user = (from u in context.Users
                            where u.Username == username
                            select u).FirstOrDefault();

                if (user == null)
                {
                    return Ok("The username '" + username + "' is not in use!");
                }
                else
                {
                    return NotFound("The username '" + username + "' is already in use!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while checking if the username is already exists. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // GET: api/user/userid
        // Get user ID for the public portfolio
        [HttpGet]
        [Route("userid/{username}")]
        public ActionResult GetUserId(string username)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                int id = (from u in context.Users
                          where u.Username == username
                          select u.UserId).FirstOrDefault();

                if (id != 0)
                {
                    return Ok(id);
                }
                else
                {
                    return NotFound("Could not found any user with username " + username);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem! Message: " + ex.InnerException);
            }
            finally
            {
                context.Dispose();
            }
        }

        // GET: api/user/auth
        // Check if the user is authenticated
        [HttpGet]
        [Route("auth")]
        public ActionResult CheckAuth([FromHeader(Name = "Authorization")] string header)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Diffs a token from header
            string[] headerParts = header.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string token = headerParts[1];

            try
            {
                if (token != "null")
                {
                    // Decoding the token to set the user ID to variable
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadJwtToken(token);

                    var id = int.Parse(jsonToken.Claims.First(claim => claim.Type == "nameid").Value);

                    // If user have the same token in database, authentication is succeesed
                    Users login = context.Users.Find(id);

                    if (token == login.JwtToken)
                    {
                        return Ok("You are authenticated!");
                    }
                    else
                    {
                        return NotFound("UnauthorizedError");
                    }
                }
                else
                {
                    return BadRequest("Problem");
                }
            }
            catch (Exception)
            {
                return BadRequest("Problem");
            }
            finally
            {
                context.Dispose();
            }
        }

        // POST: api/user/create
        // Create a new user
        [HttpPost]
        [Route("create")]
        public ActionResult AddNewUser([FromBody] Users newUser)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                context.Users.Add(newUser);

                // Add a SAS token and the portfolio content with default values to a new user
                if (context.SaveChanges() > 0)
                {
                    AddSasToUser(context, newUser.Username);
                    PortfolioContentController.AddDefaultContent(newUser.Username);
                    PortfolioContentController.AddDefaultEmails(newUser.Username);
                }

                return Ok("New user has created and the portfolio content with default values has added!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding a new user. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // Checks if old password is written correctly
        public bool CheckPassword(Users user, string password)
        {
            string oldPassword = user.Password;

            if (oldPassword == password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Creates a 20 chars long key for token
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789=?!_";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // POST: api/user/check
        // Create a new JWT Token for user and send it to database
        [HttpPost]
        [Route("check")]
        public ActionResult CheckLogin([FromBody] Users login)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Search right user from database
                var user = (from u in context.Users
                            where u.Username == login.Username &&
                            u.Password == login.Password
                            select u).FirstOrDefault();

                if (user != null)
                {
                    // Create a new SAS token for Azure Blob Storage
                    string sasToken = AddSasToUser(context, user.Username);

                    // Random key for token
                    string secretKey = RandomString(20);

                    // Instance from JwtSecurityTokenHandler
                    var tokenHandler = new JwtSecurityTokenHandler();

                    // Description for token
                    var tokenDescription = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                            new Claim(ClaimTypes.Name, user.Username)
                        }),
                        Expires = DateTime.Now.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256),
                    };

                    // Creates a token based to description
                    var token = tokenHandler.CreateToken(tokenDescription);
                    string tokenString = tokenHandler.WriteToken(token);

                    // Save token to the database
                    user.JwtToken = tokenString;

                    if (context.SaveChanges() > 0 && sasToken != null)
                    {
                        return Ok(tokenString + "|" + sasToken);
                    }
                    else
                    {
                        Exception ex = new Exception();
                        return BadRequest("Failed to create tokens! Error message: " + ex.InnerException);
                    }
                }
                else
                {
                    return NotFound("User not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong! Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // PUT: api/user/{userId}
        // Change users password
        [HttpPut]
        [Route("{id}")]
        public ActionResult ChangePassword(int id, [FromBody] Passwords passwords)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Searching right user with ID
            var user = context.Users.Find(id);

            // If old password is correct, password will be changed
            if (CheckPassword(user, passwords.OldPassword))
            {
                try
                {
                    user.Password = passwords.NewPassword;
                    context.SaveChanges();

                    return Ok("Password updated succesfully!");
                }
                catch (Exception ex)
                {
                    return BadRequest("Problem detected while deleting user. Error message: " + ex.Message);
                }
                finally
                {
                    context.Dispose();
                }
            }
            else
            {
                return NotFound("Wrong old password.");
            }
        }

        // DELETE: api/user/{userId}
        // Delete an account
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteAccount(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                if (id != 0)
                {
                    if (PortfolioContentController.DeletePortfolio(id))
                    {
                        // Searching right user with ID
                        var user = context.Users.Find(id);

                        context.Remove(user);
                        context.SaveChanges();
                    }
                }

                return Ok("Account and all content has deleted succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while deleting user. Error message: " + ex.InnerException);
            }
            finally
            {
                context.Dispose();
            }
        }

        public static string accessKey = "<Microsoft Azure Storage Account Access Key>";

        // GET: api/user/sas/
        // Delete an account
        [HttpGet]
        [Route("sas")]
        public ActionResult GetSasForPublicPortfolio()
        {
            // Account name and access key
            var storageAccountName = "webportfolio";

            var connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, accessKey);
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            // Generate a SAS token for the user's container/object to Webportfolio's Storage Account
            SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
            {
                Permissions = SharedAccessAccountPermissions.Read,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Object,
                SharedAccessStartTime = DateTime.UtcNow.AddHours(-1),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                Protocols = SharedAccessProtocol.HttpsOnly
            };
            string sasToken = storageAccount.GetSharedAccessSignature(policy);

            return Ok(sasToken);
        }

        private string AddSasToUser(WebPortfolioContext context, string username)
        {
            // Search the user which has added recently
            Users addedUser = (from u in context.Users
                               where u.Username == username
                               select u).FirstOrDefault();

            // Account name and access key
            var storageAccountName = "webportfolio";

            var connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, accessKey);
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            // Generate a SAS token for the user's container/object to Webportfolio's Storage Account
            SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
            {
                Permissions = SharedAccessAccountPermissions.Write |
                              SharedAccessAccountPermissions.Create |
                              SharedAccessAccountPermissions.Read |
                              SharedAccessAccountPermissions.Delete |
                              SharedAccessAccountPermissions.Add |
                              SharedAccessAccountPermissions.List,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Object | SharedAccessAccountResourceTypes.Container,
                SharedAccessStartTime = DateTime.UtcNow.AddHours(-1),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(8),
                Protocols = SharedAccessProtocol.HttpsOnly,
            };
            string sasToken = storageAccount.GetSharedAccessSignature(policy);

            // Save the SAS token to database
            addedUser.SasToken = sasToken;
            context.SaveChanges();

            return sasToken;
        }

        // POST: api/user/passwordreset/{email}
        // Send a message for password reset
        [HttpPost]
        [Route("passwordreset/{email}")]
        public ActionResult SendResetPasswordEmail(string email)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                Users user = (from e in context.Emails
                              where e.EmailAddress == email
                              join u in context.Users
                              on e.UserId equals u.UserId
                              select u).FirstOrDefault();

                if (user != null)
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient client = new SmtpClient("<outgoing mail server>");

                    mail.From = new MailAddress("<Sending email address>");
                    mail.To.Add(email);
                    mail.Subject = "Web Portfolio password reset";
                    mail.IsBodyHtml = true;

                    // Create a random token
                    var rng = RandomNumberGenerator.Create();

                    byte[] bytes = new byte[96];
                    rng.GetBytes(bytes);

                    string token = Convert.ToBase64String(bytes);

                    string link = "http://localhost:3000/resetpassword/" + token;
                    mail.Body = "<h3>Click the link below to reset your password</h3><br/><a href=" + link + ">" + link + "</a>";

                    client.Port = 587;
                    client.Credentials = new NetworkCredential("<Sending email address>", "<Password>");
                    client.EnableSsl = true;

                    client.Send(mail);

                    user.PasswordResetToken = token;
                    context.SaveChanges();

                    return Ok("Email has sent succesfully!");
                }
                else
                {
                    return NotFound("Incorrect email address!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while sending an email. Error message: " + ex.InnerException);
            }
            finally
            {
                context.Dispose();
            }
        }

        // POST: api/user/passwordreset
        // Reset a user´s password
        [HttpPost]
        [Route("passwordreset")]
        public ActionResult ResetPassword([FromBody] Passwords passwords)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                Users user = (from u in context.Users
                              where u.PasswordResetToken == passwords.ResetToken
                              select u).FirstOrDefault();

                if (user != null)
                {
                    user.Password = passwords.NewPassword;
                    user.PasswordResetToken = null;
                    context.SaveChanges();

                    return Ok("Password reset success!");
                }
                else
                {
                    return NotFound("The reset token do not match!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while reseting a password. Error message: " + ex.InnerException);
            }
            finally
            {
                context.Dispose();
            }
        }

        // POST: api/user/checkresettoken
        // Check the token for password reset
        [HttpPost]
        [Route("checkresettoken")]
        public ActionResult CheckToken([FromBody] Passwords passwords)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                Users user = (from u in context.Users
                              where u.PasswordResetToken == passwords.ResetToken
                              select u).FirstOrDefault();

                if (user != null)
                {
                    return Ok("Token check passed!");
                }
                else
                {
                    return NotFound("Invalid token!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while reseting a password. Error message: " + ex.InnerException);
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}