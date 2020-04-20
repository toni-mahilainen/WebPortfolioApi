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
    public class PortfolioContentController : ControllerBase
    {
        // GET: api/portfoliocontent/content/{userId}
        // Get all portfolio content
        [HttpGet]
        [Route("content/{id}")]
        public ActionResult GetAllContent(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var content = (from pc in context.PortfolioContent
                               where pc.UserId == id
                               select new 
                               {
                                   pc.Firstname,
                                   pc.Lastname,
                                   pc.Birthdate,
                                   pc.City,
                                   pc.Country,
                                   pc.Phonenumber,
                                   pc.Punchline,
                                   pc.BasicKnowledge,
                                   pc.Education,
                                   pc.WorkHistory,
                                   pc.LanguageSkills,
                               }).ToList();

                return Ok(content);
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while getting portfolio content for user " + id + ". Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // GET: api/portfoliocontent/emails/{portfolioId}
        // Get all users email addresses
        [HttpGet]
        [Route("emails/{id}")]
        public ActionResult GetAllEmails(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var emails = (from e in context.Emails
                               where e.PortfolioId == id
                               select e.EmailAddress).ToList();

                return Ok(emails);
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while getting email addresses for user " + id + ". Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // GET: api/portfoliocontent/images/{userId}
        // Get all users images
        [HttpGet]
        [Route("images/{id}")]
        public ActionResult GetAllImages(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var images = (from i in context.ImageUrls
                              where i.UserId == id
                              select new 
                              { 
                                  i.TypeId, 
                                  i.Url 
                              }).ToList();

                return Ok(images);
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while getting images for user " + id + ". Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // POST: api/portfoliocontent/content/{userId}
        // Add new portfolio content
        [HttpPost]
        [Route("content/{id}")]
        public ActionResult AddContent(int id, [FromBody] AllContent newContent)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Adding content to database (except emails)
                PortfolioContent newPortfolio = new PortfolioContent
                {
                    UserId = id,
                    Firstname = newContent.Firstname,
                    Lastname = newContent.Lastname,
                    Birthdate = newContent.Birthdate,
                    City = newContent.City,
                    Country = newContent.Country,
                    Phonenumber = newContent.Phonenumber,
                    Punchline = newContent.Punchline,
                    BasicKnowledge = newContent.BasicKnowledge,
                    Education = newContent.Education,
                    WorkHistory = newContent.WorkHistory,
                    LanguageSkills = newContent.LanguageSkills
                };

                context.PortfolioContent.Add(newPortfolio);
                context.SaveChanges();

                // Adding emails to database
                // Searching for right portfolio ID
                int portfolioId = (from pc in context.PortfolioContent
                                   where pc.UserId == id
                                   select pc.PortfolioId).FirstOrDefault();

                // Make an array for new email addresses and add them to database
                var emailsArray = newContent.Emails;

                for (int i = 0; i < emailsArray.Length; i++)
                {
                    Emails emails = new Emails();
                    emails.PortfolioId = portfolioId;
                    emails.EmailAddress = emailsArray[i];
                    context.Emails.Add(emails);
                    context.SaveChanges();
                }

                return Ok("New content has saved!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding portfolio content for user " + newContent.Firstname + " " + newContent.Lastname + ". Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // POST: api/portfoliocontent/images/{userId}
        // Add new portfolio content
        [HttpPost]
        [Route("images/{id}")]
        public ActionResult AddImages(int id, [FromBody] AllContent newContent)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Adding content to database (except emails)
                PortfolioContent newPortfolio = new PortfolioContent
                {
                    UserId = id,
                    Firstname = newContent.Firstname,
                    Lastname = newContent.Lastname,
                    Birthdate = newContent.Birthdate,
                    City = newContent.City,
                    Country = newContent.Country,
                    Phonenumber = newContent.Phonenumber,
                    Punchline = newContent.Punchline,
                    BasicKnowledge = newContent.BasicKnowledge,
                    Education = newContent.Education,
                    WorkHistory = newContent.WorkHistory,
                    LanguageSkills = newContent.LanguageSkills
                };

                context.PortfolioContent.Add(newPortfolio);
                context.SaveChanges();

                // Adding emails to database
                // Searching for last added portfolio ID
                int portfolioId = (from pc in context.PortfolioContent
                                   orderby pc.PortfolioId ascending
                                   select pc.PortfolioId).LastOrDefault();

                // Make an array for new email addresses and add them to database
                var emailsArray = newContent.Emails;


                for (int i = 0; i < emailsArray.Length; i++)
                {
                    Emails emails = new Emails();
                    emails.PortfolioId = portfolioId;
                    emails.EmailAddress = emailsArray[i];
                    context.Emails.Add(emails);
                    context.SaveChanges();
                }

                return Ok("New content has saved!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding portfolio content for user " + newContent.Firstname + " " + newContent.Lastname + ". Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}