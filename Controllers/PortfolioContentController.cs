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
        // Get all portfolio basic content
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

        // GET: api/portfoliocontent/emails/{userId}
        // Get all users email addresses
        [HttpGet]
        [Route("emails/{id}")]
        public ActionResult GetEmails(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var emails = (from e in context.Emails
                              where e.UserId == id
                              select new
                              {
                                  e.EmailId,
                                  e.EmailAddress
                              }).ToList();

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
        public ActionResult AddContent(int id, [FromBody] PortfolioContent newContent)
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

        // Add default portfolio content to new user
        static public bool AddDefaultContent(string username)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                int userId = (from u in context.Users
                              where u.Username == username
                              select u.UserId).FirstOrDefault();

                // Adding default content to database
                PortfolioContent newPortfolio = new PortfolioContent
                {
                    UserId = userId,
                    Firstname = "",
                    Lastname = "",
                    Birthdate = new DateTime(1900, 01, 01),
                    City = "",
                    Country = "",
                    Phonenumber = "",
                    Punchline = "",
                    BasicKnowledge = "",
                    Education = "",
                    WorkHistory = "",
                    LanguageSkills = ""
                };

                context.PortfolioContent.Add(newPortfolio);
                context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                context.Dispose();
            }
        }

        // Add default email addresses to the new user
        static public bool AddDefaultEmails(string username)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                int userId = (from u in context.Users
                              where u.Username == username
                              select u.UserId).FirstOrDefault();

                for (int i = 0; i < 2; i++)
                {
                    Emails emails = new Emails
                    {
                        UserId = userId,
                        EmailAddress = ""
                    };
                    context.Emails.Add(emails);
                    context.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                context.Dispose();
            }
        }

        // POST: api/portfoliocontent/emails/{userId}
        // Add email addresses to user
        [HttpPost]
        [Route("emails/{id}")]
        public ActionResult AddEmails(int id, [FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();
            
            try
            {
                // Adding emails to database
                // Converts emails json element to string
                string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);
                // Converts json string to JSON object
                var obj = JsonConvert.DeserializeObject<JObject>(json);
                // Converts object to an array
                var emailsArray = obj["Emails"].ToArray();

                for (int i = 0; i < emailsArray.Length; i++)
                {
                    Emails emails = new Emails
                    {
                        UserId = id,
                        EmailAddress = emailsArray[i]["EmailAddress"].ToString()
                    };
                    context.Emails.Add(emails);
                    context.SaveChanges();
                }

                return Ok("Emails has saved!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding portfolio content for user " + id + ". Error message: " + ex.Message);
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
                return BadRequest("Problem detected while updating portfolio content. Error message: " + ex.InnerException.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // PUT: api/portfoliocontent/emails
        // Update users email address
        [HttpPut]
        [Route("emails")]
        public ActionResult UpdateEmails([FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Updates emails to database
            // Converts emails json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);
            // Converts json string to JSON object
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            // Converts object to an array
            var emailsArray = obj["Emails"].ToArray();

            try
            {
                for (int i = 0; i < emailsArray.Length; i++)
                {
                    int emailId = int.Parse(emailsArray[i]["EmailId"].ToString());

                    Emails oldEmail = (from e in context.Emails
                                       where e.EmailId == emailId
                                       select e).FirstOrDefault();

                    oldEmail.EmailAddress = emailsArray[i]["EmailAddress"].ToString();
                    context.SaveChanges();
                }

                return Ok("Emails updated succesfully!");
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
                if (id != 0)
                {
                    bool mailBool = false;
                    bool messageBool = false;
                    bool imageBool = false;
                    bool linkBool = false;
                    bool skillBool = false;

                    // Searching right emails with user ID
                    var emailIdArray = (from e in context.Emails
                                        where e.UserId == id
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

                        mailBool = true;
                    }
                    else
                    {
                        mailBool = true;
                    }

                    // Searching right questbook messages with user ID
                    var messageIdArray = (from qm in context.QuestbookMessages
                                          where qm.UserId == id
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
                    else
                    {
                        messageBool = true;
                    }

                    // Searching images with user ID
                    var imageIdArray = (from iu in context.ImageUrls
                                        where iu.UserId == id
                                        select iu.UrlId).ToArray();

                    int urlArrayCount = imageIdArray.Count();

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
                    else
                    {
                        imageBool = true;
                    }

                    // Searching social media links with user ID
                    var linkIdArray = (from sml in context.SocialMediaLinks
                                       where sml.UserId == id
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
                    else
                    {
                        linkBool = true;
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
                    else
                    {
                        skillBool = true;
                    }

                    // Searching right portfolio with user ID
                    int portfolioId = (from pc in context.PortfolioContent
                                       where pc.UserId == id
                                       select pc.PortfolioId).FirstOrDefault();

                    // At the end, search the right portfolio and remove it
                    if ((mailBool = true) && (messageBool = true) && (imageBool = true) && (linkBool = true) && (skillBool = true))
                    {
                        PortfolioContent portfolio = context.PortfolioContent.Find(portfolioId);

                        context.Remove(portfolio);
                        context.SaveChanges();
                    }
                    else
                    {
                        return false;
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