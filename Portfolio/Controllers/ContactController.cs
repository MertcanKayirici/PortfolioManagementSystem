using System;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Controller responsible for handling contact form submissions
    /// from the public-facing website.
    /// Stores incoming messages in the database.
    /// </summary>
    public class ContactController : Controller
    {
        // Database context used for message operations.
        PortfolioDbEntities db = new PortfolioDbEntities();

        /// <summary>
        /// Receives and processes contact form submissions.
        /// Creates a new message record and stores it in the database.
        /// </summary>
        /// <param name="name">Sender's name.</param>
        /// <param name="email">Sender's email address.</param>
        /// <param name="subject">Message subject.</param>
        /// <param name="message">Message content.</param>
        /// <returns>Redirects to the home page after successful submission.</returns>
        [HttpPost]
        public ActionResult Send(string name, string email, string subject, string message)
        {
            // Create a new message entity from submitted form data.
            var newMessage = new Messages
            {
                Name = name,
                Email = email,
                Subject = subject,
                MessageContent = message,
                SendDate = DateTime.Now,
                IsRead = false // New messages are marked as unread by default.
            };

            // Save message to database.
            db.Messages.Add(newMessage);
            db.SaveChanges();

            // Store success message for UI feedback.
            TempData["Success"] = "Mesajınız gönderildi!";

            return RedirectToAction("Index", "Home");
        }
    }
}