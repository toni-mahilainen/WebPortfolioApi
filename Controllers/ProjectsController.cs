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
                context.Dispose();
            }
        }

        // POST: api/projects/{skillId}
        // Add/update users projects for specific skill
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddOrUpdateProjects(int id, [FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();

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

                        context.Projects.Add(project);
                        context.SaveChanges();
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
                context.Dispose();
            }
        }

        // Update users single project for specific skill
        static public bool UpdateProject(int id, Projects newProject)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Get a project that needs to update and new data is placed to database
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