using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebPortfolioCoreApi.Models;

namespace WebPortfolioCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialMediaController : ControllerBase
    {
        // GET: api/socialmedia/{userId}
        // Get all social media links for users portfolio
        [HttpGet]
        [Route("{id}")]
        public ActionResult GetLinks(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var allLinks = (from sml in context.SocialMediaLinks
                                where sml.UserId == id
                                select new
                                {
                                    sml.LinkId,
                                    sml.UserId,
                                    sml.ServiceId,
                                    sml.Link
                                }).ToList();

                return Ok(allLinks);
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while getting links for portfolio " + id + ". Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // POST: api/socialmedia/{userId}
        // Add new social media service links for specific portfolio
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddLinks(int id, [FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Converts json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);

            // Converts json string to JSON object
            var obj = JsonConvert.DeserializeObject<JObject>(json);

            try
            {
                int servicesArrayCount = obj["Services"].Count();
                for (int i = 0; i < servicesArrayCount; i++)
                {
                    int serviceId = int.Parse(obj["Services"][i]["ServiceId"].ToString());

                    // Check if user already have a link for service. If not an addition is made to the database
                    var linkCheck = (from sml in context.SocialMediaLinks
                                     where sml.ServiceId == serviceId
                                     select new
                                     {
                                         sml.Service
                                     }).ToList();

                    if (linkCheck.Count() == 0)
                    {
                        // Placed new link info to an object and adding it to database
                        SocialMediaLinks link = new SocialMediaLinks
                        {
                            UserId = id,
                            ServiceId = serviceId,
                            Link = obj["Services"][i]["Link"].ToString()
                        };

                        context.SocialMediaLinks.Add(link);
                        context.SaveChanges();
                    }
                }

                return Ok("New links has added to portfolio!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding a new link. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // PUT: api/socialmedia/{linkId}
        // Update a single link for social media service
        [HttpPut]
        [Route("{id}")]
        public ActionResult UpdateLink(int id, [FromBody] SocialMediaLinks newLink)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Get a link that need to update. New data is placed to database
                SocialMediaLinks oldLink = (from sml in context.SocialMediaLinks
                                            where sml.LinkId == id
                                            select sml).FirstOrDefault();

                oldLink.ServiceId = newLink.ServiceId;
                oldLink.Link = newLink.Link;
                context.SaveChanges();

                return Ok("Link has updated succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while updating a link. Link ID: " + id + ". Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // DELETE: api/socialmedia/{linkId}
        // Delete a link
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteLink(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var link = context.SocialMediaLinks.Find(id);

                if (link != null)
                {
                    context.Remove(link);
                    context.SaveChanges();
                }

                return Ok("Link deleted succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while deleting the link. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // Delete all message
        static public bool DeleteAllLinks(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching right link with ID
                var link = context.SocialMediaLinks.Find(id);

                // Deletion from the database is performed
                if (link != null)
                {
                    context.Remove(link);
                    context.SaveChanges();
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