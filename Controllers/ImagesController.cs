using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebPortfolioCoreApi.Models;

namespace WebPortfolioCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        // GET: api/images/{userId}
        // Get all users image url´s 
        [HttpGet]
        [Route("{id}")]
        public ActionResult GetImages(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                var images = (from i in context.ImageUrls
                              where i.UserId == id
                              select new
                              {
                                  i.TypeId,
                                  i.Url
                              }).ToList();

                return Ok(images);
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while getting images for user " + id + ". Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // POST: api/images/{userId}
        // Add users image url´s to database
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddImages(int id, [FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Converts json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);

            // Converts json string to dataset
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);

            try
            {
                if (dataSet != null)
                {
                    // Get through every table from dataset
                    // Tables includes type ID and new URL for image
                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        DataRow row = dataTable.Rows[0];

                        int typeId = int.Parse(row["TypeID"].ToString());
                        string url = row["Url"].ToString();

                        // Adds image urls to database
                        ImageUrls newImage = new ImageUrls
                        {
                            UserId = id,
                            TypeId = typeId,
                            Url = url
                        };

                        context.ImageUrls.Add(newImage);
                        context.SaveChanges();
                    }
                }

                return Ok("Images has added succesfully!");
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

        // PUT: api/images/{userId}
        // Update users image url´s
        [HttpPut]
        [Route("{id}")]
        public ActionResult UpdateImages(int id, [FromBody] JsonElement jsonElement)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            // Converts json element to string
            string json = System.Text.Json.JsonSerializer.Serialize(jsonElement);

            // Converts json string to dataset
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);

            try
            {
                if (dataSet != null)
                {
                    // Get through every table from dataset
                    // Tables includes type ID and new URL for image
                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        DataRow row = dataTable.Rows[0];

                        int typeId = int.Parse(row["TypeID"].ToString());
                        string newUrl = row["Url"].ToString();

                        ImageUrls oldImage = (from iu in context.ImageUrls
                                              where iu.UserId == id && iu.TypeId == typeId
                                              select iu).FirstOrDefault();

                        // If image url has changed, it will be updated to database
                        if (newUrl != oldImage.Url)
                        {
                            oldImage.Url = newUrl;
                            context.SaveChanges();
                        }
                    }

                    return Ok("Images updated succesfully!");
                }
                else
                {
                    return NotFound("Content not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while updating images. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // Delete all message
        static public bool DeleteAllImages(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching right message with ID
                var image = context.ImageUrls.Find(id);

                // Deletion from the database is performed
                if (image != null)
                {
                    context.Remove(image);
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