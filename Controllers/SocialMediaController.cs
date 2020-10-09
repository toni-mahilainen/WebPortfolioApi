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
        public WebPortfolioContext _context;

        public SocialMediaController(WebPortfolioContext context)
        {
            _context = context;
        }

        // GET: api/socialmedia/{userId}
        // Get all links to social media for user
        [HttpGet]
        [Route("{id}")]
        public ActionResult GetLinks(int id)
        {
            try
            {
                var allLinks = (from sml in _context.SocialMediaLinks
                                where sml.UserId == id
                                select new
                                {
                                    sml.LinkId,
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
                _context.Dispose();
            }
        }

        // POST: api/socialmedia/{userId}
        // Add a new social media service links or update existing ones for user
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddOrUpdateLinks(int id, [FromBody] JsonElement jsonElement)
        {
            // Converts json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);

            // Converts json string to JSON object
            var obj = JsonConvert.DeserializeObject<JObject>(json);

            try
            {
                // Converts nested "Services" JSON object to an array and save count of an array to variable
                var servicesArray = obj["Services"].ToArray();
                int servicesArrayCount = servicesArray.Count();

                // Adds as many links as the count of an array indicates
                for (int a = 0; a < servicesArrayCount; a++)
                {
                    int linkId = int.Parse(servicesArray[a]["LinkId"].ToString());

                    // If the link is new (linkId == 0), a create operation is made to database. Otherwise an update
                    if (linkId == 0)
                    {
                        SocialMediaLinks link = new SocialMediaLinks
                        {
                            UserId = id,
                            ServiceId = int.Parse(servicesArray[a]["ServiceId"].ToString()),
                            Link = servicesArray[a]["Link"].ToString()
                        };

                        _context.SocialMediaLinks.Add(link);
                        _context.SaveChanges();
                    }
                    else
                    {
                        // Updates the link
                        SocialMediaLinks link = new SocialMediaLinks
                        {
                            UserId = id,
                            ServiceId = int.Parse(servicesArray[a]["ServiceId"].ToString()),
                            Link = servicesArray[a]["Link"].ToString()
                        };

                        if (!UpdateLink(linkId, link))
                        {
                            return BadRequest("Problem detected while updating social media link. Link ID: " + linkId);
                        }
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
                _context.Dispose();
            }
        }

        // Update a single link for social media service
        public bool UpdateLink(int id, SocialMediaLinks newLink)
        {
            try
            {
                // Get a link that need to update. New data is placed to database
                SocialMediaLinks oldLink = (from sml in _context.SocialMediaLinks
                                            where sml.LinkId == id
                                            select sml).FirstOrDefault();

                oldLink.ServiceId = newLink.ServiceId;
                oldLink.Link = newLink.Link;
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        // DELETE: api/socialmedia/{linkId}
        // Delete the link
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteLink(int id)
        {
            try
            {
                var link = _context.SocialMediaLinks.Find(id);

                if (link != null)
                {
                    _context.Remove(link);
                    _context.SaveChanges();
                }

                return Ok("Link deleted succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while deleting the link. Error message: " + ex.Message);
            }
            finally
            {
                _context.Dispose();
            }
        }

        // Delete all links
        public bool DeleteAllLinks(int id)
        {
            try
            {
                // Searching right link with ID
                var link = _context.SocialMediaLinks.Find(id);

                // Deletion from the database is performed
                if (link != null)
                {
                    _context.Remove(link);
                    _context.SaveChanges();
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