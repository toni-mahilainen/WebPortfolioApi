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
        // Add users skills to database
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddSkillAndProjects(int id, [FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Converts json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);

            // Converts json string to JSON object
            var obj = JsonConvert.DeserializeObject<JObject>(json);

            try
            {
                // Adds new skill to database
                Skills skill = new Skills
                {
                    UserId = id,
                    Skill = obj["Skill"]["SkillName"].ToString(),
                    SkillLevel = int.Parse(obj["Skill"]["SkillLevel"].ToString())
                };

                context.Skills.Add(skill);
                context.SaveChanges();

                // Get last added skill ID for specific user
                int skillId = (from s in context.Skills
                               where s.UserId == id
                               orderby s.SkillId ascending
                               select s.SkillId).Last();

                // Converts nested "Projects" JSON object to array and save count of an array to variable
                var projectsArray = obj["Projects"].ToArray();
                int arrayCount = projectsArray.Count();

                // Adds as many projects as the count of an array indicates
                for (int i = 0; i < arrayCount; i++)
                {
                    Projects project = new Projects
                    {
                        SkillId = skillId,
                        Name = obj["Projects"][i]["Name"].ToString(),
                        Link = obj["Projects"][i]["Link"].ToString(),
                        Description = obj["Projects"][i]["Description"].ToString()
                    };
                    context.Projects.Add(project);
                    context.SaveChanges();
                }

                return Ok("New skill has saved!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding image for user " + id + ". Error message: " + ex.Message);
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
        public ActionResult UpdateSkill(int id, [FromBody] Skills newSkill)
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

                return Ok("Skill has updated succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while updating a skill. Skill ID: " + id + ". Error message: " + ex.Message);
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
        public ActionResult DeleteSkillAndProjects(int id)
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
    }
}