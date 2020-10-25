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
        public WebPortfolioContext _context;

        public DocumentationController(WebPortfolioContext context)
        {
            _context = context;
        }

        // GET: api/documentation/
        // Get REST API documentation
        [HttpGet]
        [Route("")]
        public ActionResult GetDocumentation()
        {
            try
            {
                // Searching the doc
                var doc = (from d in _context.Documentation
                           select d).ToList();

                return Ok(doc);
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while searching messages for questbook. Error message: " + ex.Message);
            }
            finally
            {
                _context.Dispose();
            }
        }
    }
}