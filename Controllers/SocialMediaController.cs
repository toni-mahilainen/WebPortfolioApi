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
        public ActionResult GetSocialMediaLinks(int id)
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
    }
}