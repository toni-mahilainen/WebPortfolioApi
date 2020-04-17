﻿using System;
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
    public class QuestbookController : ControllerBase
    {
        // GET: api/questbook/
        // Get all visitors + messages
        [HttpGet]
        [Route("{id}")]
        public ActionResult GetAllMessages(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching right portfolio with user ID
                var portfolioId = (from p in context.PortfolioContent
                                 where p.UserId == id
                                 select p.PortfolioId).FirstOrDefault();

                // Searching all visitors/messages with portfolio ID
                var questbookContent = (from q in context.QuestbookMessages
                                        where q.PortfolioId == portfolioId
                                        join v in context.Visitors
                                        on q.VisitorId equals v.VisitorId
                                        select new 
                                        { 
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

        // POST: api/questbook/
        // Add new message
        [HttpPost]
        [Route("{id}")]
        public ActionResult AddNewMessage(int id, [FromBody] NewMessage newMessage)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Placed visitor info to an object and adding it to database
                Visitors visitor = new Visitors
                {
                    Firstname = newMessage.VisitorFirstname,
                    Lastname = newMessage.VisitorLastname,
                    Company = newMessage.VisitorCompany
                };

                context.Visitors.Add(visitor);
                context.SaveChanges();

                // Searching last added visitor ID and adding other message and other information to database
                int visitorId = (from v in context.Visitors
                                 orderby v.VisitorId ascending
                                 select v.VisitorId).LastOrDefault();

                QuestbookMessages messageAndOthers = new QuestbookMessages
                {
                    PortfolioId = id,
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

        // DELETE: api/questbook/{id}
        // Delete message
        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteAccount(int id)
        {
            WebPortfolioContext context = new WebPortfolioContext();

            try
            {
                // Searching right user with ID
                var user = context.Users.Find(id);

                if (user != null)
                {
                    context.Remove(user);
                    context.SaveChanges();
                }

                return Ok("Account deleted succesfully!");
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
    }
}