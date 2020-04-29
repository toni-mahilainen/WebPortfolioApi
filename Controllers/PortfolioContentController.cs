using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public ActionResult GetContent(int id)
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
        public ActionResult GetEmails(int id)
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
                    Emails emails = new Emails
                    {
                        PortfolioId = portfolioId,
                        EmailAddress = emailsArray[i]
                    };
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

        // PUT: api/portfoliocontent/content/{userId}
        // Update users portfolio content
        [HttpPut]
        [Route("content/{id}")]
        public ActionResult UpdateContent(int id, [FromBody] PortfolioContent newContent)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Searching right portfolio with ID
            PortfolioContent portfolio = (from pc in context.PortfolioContent
                                          where pc.UserId == id
                                          select pc).FirstOrDefault();
            try
            {
                if (portfolio != null)
                {
                    portfolio.Firstname = newContent.Firstname;
                    portfolio.Lastname = newContent.Lastname;
                    portfolio.Birthdate = newContent.Birthdate;
                    portfolio.City = newContent.City;
                    portfolio.Country = newContent.Country;
                    portfolio.Phonenumber = newContent.Phonenumber;
                    portfolio.Punchline = newContent.Punchline;
                    portfolio.BasicKnowledge = newContent.BasicKnowledge;
                    portfolio.Education = newContent.Education;
                    portfolio.WorkHistory = newContent.WorkHistory;
                    portfolio.LanguageSkills = newContent.LanguageSkills;
                    context.SaveChanges();

                    return Ok("Portfolio updated succesfully!");
                }
                else
                {
                    return NotFound("Not found any portfolio with user ID: " + id);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while updating portfolio content. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // PUT: api/portfoliocontent/
        // Update users email address
        [HttpPut]
        [Route("")]
        public ActionResult UpdateEmail([FromBody] Email email)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                if (email != null)
                {
                    // updating emails to database
                    Emails oldEmail = (from e in context.Emails
                                       where e.EmailId == email.EmailId
                                       select e).FirstOrDefault();

                    oldEmail.EmailAddress = email.NewEmailAddress;
                    context.SaveChanges();

                    return Ok("Email address updated succesfully!");
                }
                else
                {
                    return NotFound("Not found any email with email ID: " + email.EmailId);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while updating email address. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // Delete users portfolio and all content
        static public bool DeletePortfolio(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching right portfolio with user ID
                int portfolioId = (from pc in context.PortfolioContent
                                   where pc.UserId == id
                                   select pc.PortfolioId).FirstOrDefault();

                if (portfolioId != 0)
                {
                    bool messageBool;
                    bool imageBool;
                    bool linkBool;
                    bool skillBool;

                    // Searching right emails with portfolio ID
                    var emailIdArray = (from e in context.Emails
                                        where e.PortfolioId == portfolioId
                                        select e.EmailId).ToArray();

                    int emailArrayCount = emailIdArray.Count();

                    if (emailArrayCount > 0)
                    {
                        for (int i = 0; i < emailIdArray.Length; i++)
                        {
                            Emails email = context.Emails.Find(emailIdArray[i]);

                            context.Remove(email);
                            context.SaveChanges();
                        }
                    }

                    // Searching right questbook messages with portfolio ID
                    var messageIdArray = (from qm in context.QuestbookMessages
                                          where qm.PortfolioId == portfolioId
                                          select qm.MessageId).ToArray();

                    int messageArrayCount = messageIdArray.Count();

                    if (messageArrayCount > 0)
                    {
                        for (int i = 0; i < messageArrayCount;)
                        {
                            int messageId = messageIdArray[i];

                            messageBool = QuestbookController.DeleteAllMessages(messageId);

                            if (messageBool)
                            {
                                i++;
                            }
                        }
                    }

                    // Searching images with user ID
                    var imageIdArray = (from iu in context.ImageUrls
                                        where iu.UserId == id
                                        select iu.UrlId).ToArray();

                    int urlArrayCount = messageIdArray.Count();

                    if (urlArrayCount > 0)
                    {
                        for (int i = 0; i < urlArrayCount;)
                        {
                            int urlId = imageIdArray[i];

                            imageBool = ImagesController.DeleteAllImages(urlId);

                            if (imageBool)
                            {
                                i++;
                            }
                        }
                    }

                    // Searching social media links with portfolio ID
                    var linkIdArray = (from sml in context.SocialMediaLinks
                                        where sml.UserId == portfolioId
                                        select sml.LinkId).ToArray();

                    int linkArrayCount = linkIdArray.Count();

                    if (linkArrayCount > 0)
                    {
                        for (int i = 0; i < linkArrayCount;)
                        {
                            int linkId = linkIdArray[i];

                            linkBool = SocialMediaController.DeleteAllLinks(linkId);

                            if (linkBool)
                            {
                                i++;
                            }
                        }
                    }

                    // Searching social media links with portfolio ID
                    var skillIdArray = (from s in context.Skills
                                       where s.UserId == id
                                       select s.SkillId).ToArray();

                    int skillArrayCount = skillIdArray.Count();

                    if (skillArrayCount > 0)
                    {
                        for (int i = 0; i < skillArrayCount;)
                        {
                            int skillId = skillIdArray[i];

                            skillBool = SkillsController.DeleteAllSkillAndProjects(skillId);

                            if (skillBool)
                            {
                                i++;
                            }
                        }
                    }

                    // At the end, search the right portfolio and remove it
                    if ((messageBool = true) && (imageBool = true) && (linkBool = true) && (skillBool = true))
                    {
                        PortfolioContent portfolio = context.PortfolioContent.Find(portfolioId);

                        context.Remove(portfolio);
                        context.SaveChanges();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}