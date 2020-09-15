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
        // Add/update users skills to database
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddOrUpdateSkills(int id, [FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Converts json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);

            // Converts json string to JSON object
            var obj = JsonConvert.DeserializeObject<JObject>(json);

            try
            {
                // Converts nested "Skills"-JSON object to an array and save count of an array to variable
                var skillsArray = obj["Skills"].ToArray();
                int skillArrayCount = skillsArray.Count();

                // Adds as many skills as the count of an array indicates
                for (int i = 0; i < skillArrayCount; i++)
                {
                    int skillId = int.Parse(skillsArray[i]["SkillId"].ToString());
                    // If skill is new (skillId == 0), creation is made to database. Otherwise an update
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
                    }
                    else
                    {
                        // Updates the skill
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
                }

                return Ok("Skills has saved!");
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

        // Update users skills
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
        // Delete the skill
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