using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebPortfolioCoreApi.Models;

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

        // DELETE: api/user/{id}
        // Delete an account
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteAccount(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var user = context.Users.Find(id);

                context.Remove(user);
                context.SaveChanges();
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

        // PUT: api/user/{id}
        // Change user password
        [HttpPut]
        [Route("id")]
        public ActionResult ChangePassword(int id, [FromBody] Users newPassword )
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
            finally
            {
                context.Dispose();
            }

        }
    }
}