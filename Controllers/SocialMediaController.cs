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
    public class SocialMediaController : ControllerBase
    {
        // GET: api/socialmedia/{portfolioId}
        // Get all social media links for users portfolio
        [HttpGet]
        [Route("{id}")]
        public ActionResult GetLinks(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var allLinks = (from sml in context.SocialMediaLinks
                                where sml.PortfolioId == id
                                select new 
                                {
                                    sml.LinkId,
                                    sml.PortfolioId,
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

        // POST: api/socialmedia/{portfolioId}
        // Add new social media link for specific portfolio
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddLink(int id, [FromBody] SocialMediaLinks newLink)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Check if user already have a link for service. If not an addition is made to the database
                SocialMediaLinks linkCheck = (from sml in context.SocialMediaLinks
                                              where sml.ServiceId == newLink.ServiceId
                                              select sml).FirstOrDefault();

                if (linkCheck == null)
                {
                    // Placed new link info to an object and adding it to database
                    SocialMediaLinks link = new SocialMediaLinks
                    {
                        PortfolioId = id,
                        ServiceId = newLink.ServiceId,
                        Link = newLink.Link
                    };

                    context.SocialMediaLinks.Add(link);
                    context.SaveChanges();
                }
                else
                {
                    return NotFound("Portfolio already includes a link for service.");
                }

                return Ok("New link has added to portfolio!");
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
    }
}