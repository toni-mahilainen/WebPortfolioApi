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
                                select new { 
                                    p.ProjectId,
                                    p.Name,
                                    p.Link,
                                    p.Description
                                }).ToList();

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

        // POST: api/projects/{skillId}
        // Add new project for specific skill
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddProject(int id, [FromBody] Projects newProject)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Placed new project info to an object and adding it to database
                Projects project = new Projects
                {
                    SkillId = id,
                    Name = newProject.Name,
                    Link = newProject.Link,
                    Description = newProject.Description
                };

                context.Projects.Add(project);
                context.SaveChanges();

                return Ok("New project has added to project!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding a new project. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // PUT: api/projects/{projectId}
        // Update users single project for specific skill
        [HttpPut]
        [Route("{id}")]
        static public bool UpdateProject(int id, Projects newProject)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Get a project that need to update and new data is placed to database
                Projects oldProject = (from p in context.Projects
                                       where p.ProjectId == id
                                       select p).FirstOrDefault();

                oldProject.Name = newProject.Name;
                oldProject.Link = newProject.Link;
                oldProject.Description = newProject.Description;
                context.SaveChanges();

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

        // DELETE: api/projects/{projectId}
        // Delete a project
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteProject(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var project = context.Projects.Find(id);

                if (project != null)
                {
                    context.Remove(project);
                    context.SaveChanges();
                }

                return Ok("Project deleted succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while deleting the project. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}