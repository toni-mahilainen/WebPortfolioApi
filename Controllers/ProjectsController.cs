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
    public class ProjectsController : ControllerBase
    {
        // GET: api/projects/{skillId}
        // Get all users projects for specific skill
        [HttpGet]
        [Route("{id}")]
        public ActionResult GetProjects(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var projects = (from p in context.Projects
                              where p.SkillId == id
                              select p).ToList();

                return Ok(projects);
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while getting skills for user " + id + ". Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}