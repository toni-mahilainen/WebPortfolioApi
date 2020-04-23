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

        // POST: api/portfoliocontent/images/{userId}
        // Add users images to database
        [HttpPost]
        [Route("images/{id}")]
        public ActionResult AddImages(int id, [FromBody] ImageUrls newImageInfo)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Adds image urls to database
                ImageUrls newImage = new ImageUrls
                {
                    UserId = id,
                    TypeId = newImageInfo.TypeId,
                    Url = newImageInfo.Url
                };

                context.ImageUrls.Add(newImage);
                context.SaveChanges();

                return Ok("New image has saved!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding image for user " + id + ". Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // POST: api/portfoliocontent/skill/{userId}
        // Add users skills to database
        [HttpPost]
        [Route("skill/{id}")]
        public ActionResult AddSkill(int id, [FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Converts json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);

            // Converts json string to JSON object
            var obj = JsonConvert.DeserializeObject<JObject>(json);

            try
            {
                // Adds new skill to database
                Skills skill = new Skills
                {
                    UserId = id,
                    Skill = obj["Skill"]["SkillName"].ToString(),
                    SkillLevel = int.Parse(obj["Skill"]["SkillLevel"].ToString())
                };

                context.Skills.Add(skill);
                context.SaveChanges();

                // Get last added skill ID for specific user
                int skillId = (from s in context.Skills
                               where s.UserId == id
                               orderby s.SkillId ascending
                               select s.SkillId).Last();

                // Converts nested "Projects" JSON object to array and save count of an array to variable
                var projectsArray = obj["Projects"].ToArray();
                int arrayCount = projectsArray.Count();

                // Adds as many projects as the count of an array indicates
                for (int i = 0; i < arrayCount; i++)
                {
                    Projects project = new Projects
                    {
                        SkillId = skillId,
                        Name = obj["Projects"][i]["Name"].ToString(),
                        Link = obj["Projects"][i]["Link"].ToString(),
                        Description = obj["Projects"][i]["Description"].ToString()
                    };
                    context.Projects.Add(project);
                    context.SaveChanges();
                }

                return Ok("New skill has saved!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding image for user " + id + ". Error message: " + ex.Message);
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

        // PUT: api/portfoliocontent/images/{userId}
        // Update users images
        [HttpPut]
        [Route("images/{id}")]
        public ActionResult UpdateImages(int id, [FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Converts json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);

            // Converts json string to dataset
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);

            try
            {
                if (dataSet != null)
                {
                    // Get through every table from dataset
                    // Tables includes type ID and new URL for image
                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        DataRow row = dataTable.Rows[0];

                        int typeId = int.Parse(row["TypeID"].ToString());
                        string newUrl = row["Url"].ToString();

                        ImageUrls oldImage = (from iu in context.ImageUrls
                                              where iu.UserId == id && iu.TypeId == typeId
                                              select iu).FirstOrDefault();

                        // If image url has changed, it will be updated to database
                        if (newUrl != oldImage.Url)
                        {
                            oldImage.Url = newUrl;
                            context.SaveChanges();
                        }
                    }

                    return Ok("Images updated succesfully!");
                }
                else
                {
                    return NotFound("Content not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while updating images. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // DELETE: api/portfoliocontent/{userId}
        // Delete users portfolio content
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteContent(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching right portfolio with user ID
                PortfolioContent portfolio = (from pc in context.PortfolioContent
                                              where pc.UserId == id
                                              select pc).FirstOrDefault();
                if (portfolio != null)
                {
                    // Searching right emails with portfolio ID
                    var emailIdArray = (from e in context.Emails
                                        where e.PortfolioId == portfolio.PortfolioId
                                        select e.EmailId).ToArray();

                    for (int i = 0; i < emailIdArray.Length; i++)
                    {
                        Emails email = context.Emails.Find(emailIdArray[i]);

                        context.Remove(email);
                        context.SaveChanges();
                    }

                    // Searching right questbook messages with portfolio ID
                    var messageIdArray = (from qm in context.QuestbookMessages
                                          where qm.PortfolioId == portfolio.PortfolioId
                                          select qm.MessageId).ToArray();

                    for (int i = 0; i < messageIdArray.Length; i++)
                    {
                        int messageId = messageIdArray[i];

                        QuestbookController.DeleteAllMessages(messageId);
                    }

                    context.Remove(portfolio);
                    context.SaveChanges();
                }

                return Ok("Content deleted succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while deleting portfolio. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}