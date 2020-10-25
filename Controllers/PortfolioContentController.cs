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
        public WebPortfolioContext _context;

        public PortfolioContentController(WebPortfolioContext context)
        {
            _context = context;
        }

        // GET: api/portfoliocontent/content/{userId}
        // Get all the basic content for a portfolio
        [HttpGet]
        [Route("content/{id}")]
        public ActionResult GetContent(int id)
        {
            try
            {
                var content = (from pc in _context.PortfolioContent
                               where pc.UserId == id
                               join u in _context.Users
                               on pc.UserId equals u.UserId
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
                                   u.ThemeId
                               }).ToList();

                return Ok(content);
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while getting portfolio content for user " + id + ". Error message: " + ex.Message);
            }
            finally
            {
                _context.Dispose();
            }
        }

        // GET: api/portfoliocontent/emails/{userId}
        // Get all email addresses for a user
        [HttpGet]
        [Route("emails/{id}")]
        public ActionResult GetEmails(int id)
        {
            try
            {
                var emails = (from e in _context.Emails
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
                _context.Dispose();
            }
        }

        // POST: api/portfoliocontent/content/{userId}
        // Add new portfolio content
        [HttpPost]
        [Route("content/{id}")]
        public ActionResult AddContent(int id, [FromBody] PortfolioContent newContent)
        {
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

                _context.PortfolioContent.Add(newPortfolio);
                _context.SaveChanges();

                return Ok("New content has saved!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding portfolio content for user " + newContent.Firstname + " " + newContent.Lastname + ". Error message: " + ex.Message);
            }
            finally
            {
                _context.Dispose();
            }
        }

        // Add default portfolio content to new user
        public bool AddDefaultContent(string username)
        {
            try
            {
                var user = (from u in _context.Users
                            where u.Username == username
                            select u).FirstOrDefault();

                // Adding default content to database
                PortfolioContent newPortfolio = new PortfolioContent
                {
                    UserId = user.UserId,
                    Firstname = "",
                    Lastname = "",
                    Birthdate = new DateTime(1990, 01, 01),
                    City = "",
                    Country = "",
                    Phonenumber = "",
                    Punchline = "",
                    BasicKnowledge = "",
                    Education = "",
                    WorkHistory = "",
                    LanguageSkills = ""
                };

                user.ThemeId = 1;

                _context.PortfolioContent.Add(newPortfolio);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Add default email addresses to the new user
        public bool AddDefaultEmails(string username)
        {
            try
            {
                int userId = (from u in _context.Users
                              where u.Username == username
                              select u.UserId).FirstOrDefault();

                for (int i = 0; i < 2; i++)
                {
                    Emails emails = new Emails
                    {
                        UserId = userId,
                        EmailAddress = ""
                    };
                    _context.Emails.Add(emails);
                    _context.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // POST: api/portfoliocontent/emails/{userId}
        // Add the email addresses to a user
        [HttpPost]
        [Route("emails/{id}")]
        public ActionResult AddEmails(int id, [FromBody] JsonElement jsonElement)
        {
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
                    _context.Emails.Add(emails);
                    _context.SaveChanges();
                }

                return Ok("Emails has saved!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding portfolio content for user " + id + ". Error message: " + ex.Message);
            }
            finally
            {
                _context.Dispose();
            }
        }

        // PUT: api/portfoliocontent/content/{userId}
        // Update the user's portfolio content
        [HttpPut]
        [Route("content/{id}")]
        public ActionResult UpdateContent(int id, [FromBody] PortfolioContent newContent)
        {
            // Searching right portfolio with ID
            PortfolioContent portfolio = (from pc in _context.PortfolioContent
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
                    _context.SaveChanges();

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
                _context.Dispose();
            }
        }

        // PUT: api/portfoliocontent/emails
        // Update the user's email address
        [HttpPut]
        [Route("emails")]
        public ActionResult UpdateEmails([FromBody] JsonElement jsonElement)
        {
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

                    Emails oldEmail = (from e in _context.Emails
                                       where e.EmailId == emailId
                                       select e).FirstOrDefault();

                    oldEmail.EmailAddress = emailsArray[i]["EmailAddress"].ToString();
                    _context.SaveChanges();
                }

                return Ok("Emails updated succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while updating email address. Error message: " + ex.Message);
            }
            finally
            {
                _context.Dispose();
            }
        }

        // Delete users portfolio and all content
        public bool DeletePortfolio(int id)
        {
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
                    var emailIdArray = (from e in _context.Emails
                                        where e.UserId == id
                                        select e.EmailId).ToArray();

                    int emailArrayCount = emailIdArray.Count();

                    if (emailArrayCount > 0)
                    {
                        for (int i = 0; i < emailIdArray.Length; i++)
                        {
                            Emails email = _context.Emails.Find(emailIdArray[i]);

                            _context.Remove(email);
                            _context.SaveChanges();
                        }

                        mailBool = true;
                    }
                    else
                    {
                        mailBool = true;
                    }

                    // Searching right questbook messages with user ID
                    var messageIdArray = (from qm in _context.QuestbookMessages
                                          where qm.UserId == id
                                          select qm.MessageId).ToArray();

                    int messageArrayCount = messageIdArray.Count();

                    if (messageArrayCount > 0)
                    {
                        for (int i = 0; i < messageArrayCount;)
                        {
                            int messageId = messageIdArray[i];

                            QuestbookController controller = new QuestbookController(_context);

                            messageBool = controller.DeleteAllMessages(messageId);

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
                    var imageIdArray = (from iu in _context.ImageUrls
                                        where iu.UserId == id
                                        select iu.UrlId).ToArray();

                    int urlArrayCount = imageIdArray.Count();

                    if (urlArrayCount > 0)
                    {
                        for (int i = 0; i < urlArrayCount;)
                        {
                            int urlId = imageIdArray[i];

                            ImagesController controller = new ImagesController(_context);

                            imageBool = controller.DeleteAllImages(urlId);

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
                    var linkIdArray = (from sml in _context.SocialMediaLinks
                                       where sml.UserId == id
                                       select sml.LinkId).ToArray();

                    int linkArrayCount = linkIdArray.Count();

                    if (linkArrayCount > 0)
                    {
                        for (int i = 0; i < linkArrayCount;)
                        {
                            int linkId = linkIdArray[i];

                            SocialMediaController controller = new SocialMediaController(_context);

                            linkBool = controller.DeleteAllLinks(linkId);

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
                    var skillIdArray = (from s in _context.Skills
                                        where s.UserId == id
                                        select s.SkillId).ToArray();

                    int skillArrayCount = skillIdArray.Count();

                    if (skillArrayCount > 0)
                    {
                        for (int i = 0; i < skillArrayCount;)
                        {
                            int skillId = skillIdArray[i];

                            SkillsController controller = new SkillsController(_context);

                            skillBool = controller.DeleteAllSkillAndProjects(skillId);

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
                    int portfolioId = (from pc in _context.PortfolioContent
                                       where pc.UserId == id
                                       select pc.PortfolioId).FirstOrDefault();

                    // At the end, search the right portfolio and remove it
                    if ((mailBool = true) && (messageBool = true) && (imageBool = true) && (linkBool = true) && (skillBool = true))
                    {
                        PortfolioContent portfolio = _context.PortfolioContent.Find(portfolioId);

                        _context.Remove(portfolio);
                        _context.SaveChanges();
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
        }
    }
}