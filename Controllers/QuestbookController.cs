using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebPortfolioCoreApi.Models;
using WebPortfolioCoreApi.OtherModels;

namespace WebPortfolioCoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestbookController : ControllerBase
    {
        // GET: api/questbook/{userId}
        // Get all visitors + messages
        [HttpGet]
        [Route("{id}")]
        public ActionResult GetAllMessages(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching all visitors/messages with portfolio ID
                var questbookContent = (from q in context.QuestbookMessages
                                        where q.UserId == id
                                        join v in context.Visitors
                                        on q.VisitorId equals v.VisitorId
                                        select new
                                        {
                                            q.MessageId,
                                            v.Firstname,
                                            v.Lastname,
                                            v.Company,
                                            q.VisitationTimestamp,
                                            q.Message
                                        }).ToList();

                return Ok(questbookContent);
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

        // POST: api/questbook/{userId}
        // Add a new message
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddNewMessage(int id, [FromBody] NewMessage newMessage)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Check if a visitor already exists in the database
                var existingVisitor = (from v in context.Visitors
                                       where v.Firstname == newMessage.VisitorFirstname &&
                                       v.Lastname == newMessage.VisitorLastname &&
                                       v.Company == newMessage.VisitorCompany
                                       select v).FirstOrDefault();

                int visitorId = 0;

                if (existingVisitor == null)
                {
                    // If a visitor is new, it will be added to database
                    // Placed visitor info to an object and adding it to database
                    Visitors visitor = new Visitors
                    {
                        Firstname = newMessage.VisitorFirstname,
                        Lastname = newMessage.VisitorLastname,
                        Company = newMessage.VisitorCompany
                    };

                    context.Visitors.Add(visitor);
                    context.SaveChanges();

                    // Searching for last added visitor ID and adding other message and other information to database
                    visitorId = (from v in context.Visitors
                                 orderby v.VisitorId ascending
                                 select v.VisitorId).LastOrDefault();
                }
                else
                {
                    // If a visitor already exists in the database
                    visitorId = existingVisitor.VisitorId;
                }

                // New message to database
                QuestbookMessages messageAndOthers = new QuestbookMessages
                {
                    UserId = id,
                    VisitorId = visitorId,
                    Message = newMessage.Message,
                    VisitationTimestamp = newMessage.VisitationTimestamp
                };

                context.QuestbookMessages.Add(messageAndOthers);
                context.SaveChanges();

                return Ok("New message has saved!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while adding a new message. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // DELETE: api/questbook/{messageId}
        // Delete the message
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteMessage(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching right message with ID
                var message = context.QuestbookMessages.Find(id);

                // Searching a visitor who wrote the message
                int visitorId = (from v in context.Visitors
                                 where v.VisitorId == message.VisitorId
                                 select v.VisitorId).FirstOrDefault();

                var visitor = context.Visitors.Find(visitorId);

                // Deletion from the database is performed
                if (message != null && visitor != null)
                {
                    context.Remove(message);
                    context.Remove(visitor);
                    context.SaveChanges();
                }

                return Ok("Message deleted succesfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("Problem detected while deleting user. Error message: " + ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        // Delete all message
        static public bool DeleteAllMessages(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching right message with ID
                var message = context.QuestbookMessages.Find(id);

                // Searching a visitor who wrote the message
                int visitorId = (from v in context.Visitors
                                 where v.VisitorId == message.VisitorId
                                 select v.VisitorId).FirstOrDefault();

                var visitor = context.Visitors.Find(visitorId);

                // Deletion from the database is performed
                if (message != null && visitor != null)
                {
                    context.Remove(message);
                    context.Remove(visitor);
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