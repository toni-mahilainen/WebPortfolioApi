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
    public class ProjectsController : ControllerBase
    {
        public WebPortfolioContext _context;

        public ProjectsController(WebPortfolioContext context)
        {
            _context = context;
        }

        // GET: api/projects/{skillId}
        // Get all users projects for specific skill
        [HttpGet]
        [Route("{id}")]
        public ActionResult GetProjects(int id)
        {
            try
            {
                var projects = (from p in _context.Projects
                                where p.SkillId == id
                                select new
                                {
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
                _context.Dispose();
            }
        }

        // POST: api/projects/{skillId}
        // Add/update users projects for specific skill
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddOrUpdateProjects(int id, [FromBody] JsonElement jsonElement)
        {
            // Converts json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);

            // Converts json string to JSON object
            var obj = JsonConvert.DeserializeObject<JObject>(json);

            try
            {
                // Converts nested "Projects"-JSON object to an array and save count of an array to variable
                var projectsArray = obj["Projects"].ToArray();
                int projectsArrayCount = projectsArray.Count();

                for (int a = 0; a < projectsArrayCount; a++)
                {
                    int projectId = int.Parse(projectsArray[a]["ProjectId"].ToString());

                    // If the project is new (projectId == 0), creation is made to database. Otherwise an update
                    if (projectId == 0)
                    {
                        Projects project = new Projects()
                        {
                            SkillId = id,
                            Name = projectsArray[a]["Name"].ToString(),
                            Link = projectsArray[a]["Link"].ToString(),
                            Description = projectsArray[a]["Description"].ToString()
                        };

                        _context.Projects.Add(project);
                        _context.SaveChanges();
                    }
                    else
                    {
                        // Updates the project
                        Projects project = new Projects
                        {
                            Name = projectsArray[a]["Name"].ToString(),
                            Link = projectsArray[a]["Link"].ToString(),
                            Description = projectsArray[a]["Description"].ToString()
                        };

                        if (!UpdateProject(projectId, project))
                        {
                            return BadRequest("Problem detected while updating project for skill " + id + ".");
                        }
                    }
                }

                return Ok("New project(s) has added/updated to a skill!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding a new project. Error message: " + ex.Message);
            }
            finally
            {
                _context.Dispose();
            }
        }

        // Update users single project for specific skill
        public bool UpdateProject(int id, Projects newProject)
        {
            try
            {
                // Get a project that needs to update and new data is placed to database
                Projects oldProject = (from p in _context.Projects
                                       where p.ProjectId == id
                                       select p).FirstOrDefault();

                oldProject.Name = newProject.Name;
                oldProject.Link = newProject.Link;
                oldProject.Description = newProject.Description;
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        // DELETE: api/projects/{projectId}
        // Delete a project
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteProject(int id)
        {
            try
            {
                var project = _context.Projects.Find(id);

                if (project != null)
                {
                    _context.Remove(project);
                    _context.SaveChanges();
                }

                return Ok("Project deleted succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while deleting the project. Error message: " + ex.Message);
            }
            finally
            {
                _context.Dispose();
            }
        }
    }
}