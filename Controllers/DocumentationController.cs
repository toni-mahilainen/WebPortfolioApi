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
    public class DocumentationController : ControllerBase
    {
        // Another comment because first commit
        // GET: api/documentation/
        // Get REST API documentation
        [HttpGet]
        [Route("")]
        public ActionResult GetDocumentation()
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching doc
                var doc = (from d in context.Documentation
                           select d).ToList();

                return Ok(doc);
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while searching messages for questbook. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}