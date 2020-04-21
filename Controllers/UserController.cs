using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebPortfolioCoreApi.Models;
using WebPortfolioCoreApi.OtherModels;

namespace WebPortfolioCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // POST: api/user/
        // Add new user
        [HttpPost]
        [Route("")]
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

        // PUT: api/user/{id}
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

        // DELETE: api/user/{id}
        // Delete an account
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteAccount(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching right user with ID
                var user = context.Users.Find(id);

                if (user != null)
                {
                    context.Remove(user);
                    context.SaveChanges();
                }

                return Ok("Account deleted succesfully!");
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