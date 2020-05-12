using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebPortfolioCoreApi.Models;
using WebPortfolioCoreApi.OtherModels;

namespace WebPortfolioCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // POST: api/user/create
        // Add new user
        [HttpPost]
        [Route("create")]
        public ActionResult AddNewUser([FromBody] Users newUser)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                context.Users.Add(newUser);
                context.SaveChanges();

                return Ok("New user has created!");
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

        // PUT: api/user/{userId}
        // Change user password
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

        // Creates a 20 chars long key for token
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789=?!_";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // POST: api/user/check
        // Check login
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
                    string tokenString = "";

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
                        Expires = DateTime.Now.AddHours(2),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256),
                    };

                    // Creates a token based to description
                    var token = tokenHandler.CreateToken(tokenDescription);
                    tokenString = tokenHandler.WriteToken(token);

                    // Save token to database
                    user.Token = tokenString;
                    context.SaveChanges();

                    return Ok(tokenString);
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

        // GET: api/user/auth
        // Check auth
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

                    if (token == login.Token)
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
                return BadRequest("Problem detected while deleting user. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}