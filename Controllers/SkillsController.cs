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
    public class SkillsController : ControllerBase
    {
        // GET: api/skills/{userId}
        // Get all users skills
        [HttpGet]
        [Route("{id}")]
        public ActionResult GetSkills(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var skills = (from s in context.Skills
                              where s.UserId == id
                              select new
                              {
                                  s.SkillId,
                                  s.Skill,
                                  s.SkillLevel
                              }).ToList();

                return Ok(skills);
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

        // POST: api/skills/{userId}
        // Add users skill/projects to database
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddOrUpdateSkillsAndProjects(int id, [FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Converts json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);

            // Converts json string to JSON object
            var obj = JsonConvert.DeserializeObject<JObject>(json);

            try
            {
                // Converts nested "Skills" JSON object to an array and save count of an array to variable
                var skillsArray = obj["Skills"].ToArray();
                int skillArrayCount = skillsArray.Count();

                // Adds as many skills as the count of an array indicates
                for (int i = 0; i < skillArrayCount; i++)
                {
                    int lastAddedSkillId = 0;
                    int skillId = int.Parse(skillsArray[i]["SkillId"].ToString());
                    if (skillId == 0)
                    {
                        // Adds new skill to database
                        Skills skill = new Skills
                        {
                            UserId = id,
                            Skill = skillsArray[i]["Skill"].ToString(),
                            SkillLevel = int.Parse(skillsArray[i]["SkillLevel"].ToString())
                        };

                        context.Skills.Add(skill);
                        context.SaveChanges();

                        // Get last added skill ID for specific user
                        lastAddedSkillId = (from s in context.Skills
                                            where s.UserId == id
                                            orderby s.SkillId ascending
                                            select s.SkillId).Last();
                    }
                    else
                    {
                        Skills skill = new Skills
                        {
                            Skill = skillsArray[i]["Skill"].ToString(),
                            SkillLevel = int.Parse(skillsArray[i]["SkillLevel"].ToString())
                        };

                        if (!UpdateSkill(skillId, skill))
                        {
                            return BadRequest("Problem detected while updating the skill for user " + id + ".");
                        }
                    }

                    // Converts nested "Projects" JSON object to an array and save count of an array to variable
                    var projectsArray = obj["Skills"][i]["Projects"].ToArray();
                    int projectArrayCount = projectsArray.Count();

                    // Adds as many projects as the count of an array indicates
                    for (int a = 0; a < projectArrayCount; a++)
                    {
                        int projectId = int.Parse(projectsArray[a]["ProjectId"].ToString());
                        if (projectId == 0)
                        {
                            Projects project = new Projects();

                            if (lastAddedSkillId == 0)
                            {

                                project.SkillId = skillId;
                                project.Name = projectsArray[a]["Name"].ToString();
                                project.Link = projectsArray[a]["Link"].ToString();
                                project.Description = projectsArray[a]["Description"].ToString();

                            }
                            else
                            {
                                project.SkillId = lastAddedSkillId;
                                project.Name = projectsArray[a]["Name"].ToString();
                                project.Link = projectsArray[a]["Link"].ToString();
                                project.Description = projectsArray[a]["Description"].ToString();
                            }

                            context.Projects.Add(project);
                            context.SaveChanges();
                        }
                        else
                        {
                            Projects project = new Projects
                            {
                                Name = projectsArray[a]["Name"].ToString(),
                                Link = projectsArray[a]["Link"].ToString(),
                                Description = projectsArray[a]["Description"].ToString()
                            };

                            if (!ProjectsController.UpdateProject(projectId, project))
                            {
                                return BadRequest("Problem detected while updating project for skill " + lastAddedSkillId + ".");
                            }
                        }
                    }
                }

                return Ok("Skills/projects has saved!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding skills for user " + id + ". Error message: " + ex.InnerException.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // PUT: api/skills/{skillId}
        // Update users skills
        [HttpPut]
        [Route("{id}")]
        static public bool UpdateSkill(int id, Skills newSkill)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                Skills oldSkill = (from s in context.Skills
                                   where s.SkillId == id
                                   select s).FirstOrDefault();

                oldSkill.Skill = newSkill.Skill;
                oldSkill.SkillLevel = newSkill.SkillLevel;
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

        // DELETE: api/skills/{skillId}
        // Delete a skill
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteSkill(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Removes all projects of the skill
                Projects project = null;

                do
                {
                    project = (from p in context.Projects
                               where p.SkillId == id
                               select p).FirstOrDefault();

                    if (project != null)
                    {
                        context.Remove(project);
                        context.SaveChanges();
                    }

                } while (project != null);

                // Removes the skill
                var skill = context.Skills.Find(id);

                if (skill != null)
                {
                    context.Remove(skill);
                    context.SaveChanges();
                }

                return Ok("Skill deleted succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while deleting skill. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        static public bool DeleteAllSkillAndProjects(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Removes all projects of the skill
                Projects project = null;

                do
                {
                    project = (from p in context.Projects
                               where p.SkillId == id
                               select p).FirstOrDefault();

                    if (project != null)
                    {
                        context.Remove(project);
                        context.SaveChanges();
                    }

                } while (project != null);

                // Removes the skill
                var skill = context.Skills.Find(id);

                if (skill != null)
                {
                    context.Remove(skill);
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