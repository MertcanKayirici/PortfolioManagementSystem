using Portfolio.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Admin controller responsible for managing the contact section settings
    /// displayed on the portfolio website.
    /// </summary>
    public class AdminContactSectionController : Controller
    {
        // Database context used for ContactSectionSettings entity operations.
        PortfolioDbEntities db = new PortfolioDbEntities();

        /// <summary>
        /// Checks whether the current session belongs to an authenticated admin user.
        /// </summary>
        /// <returns>True if admin session exists; otherwise false.</returns>
        private bool IsAdmin()
        {
            return Session["Admin"] != null;
        }

        /// <summary>
        /// Verifies admin authorization before allowing access to controller actions.
        /// Returns a redirect result if the user is not authenticated.
        /// </summary>
        /// <returns>
        /// Redirects to the login page if unauthorized; otherwise returns null.
        /// </returns>
        private ActionResult CheckAdmin()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            return null;
        }

        /// <summary>
        /// Displays the edit page for the contact section.
        /// If no contact section settings record exists, a default one is created automatically.
        /// </summary>
        /// <returns>Edit view with the existing or newly created contact section settings.</returns>
        public ActionResult Edit()
        {
            var auth = CheckAdmin();
            if (auth != null) return auth;

            // Retrieve the first contact section settings record from the database.
            var setting = db.ContactSectionSettings.FirstOrDefault();

            // If no record exists, create a default contact section configuration.
            if (setting == null)
            {
                setting = new ContactSectionSettings
                {
                    Title = "Birlikte bir şeyler üretelim",
                    Description = "Yazılım, portfolyo, proje geliştirme ya da iş fırsatları hakkında benimle iletişime geçebilirsin. Mesajını gördüğümde en kısa sürede dönüş yaparım.",
                    BadgeText = "Açık, net ve hızlı iletişim",

                    Box1Icon = "bi-envelope",
                    Box1Title = "E-posta",
                    Box1Text = "mertcan@mail.com",
                    Box1Url = "mailto:mertcan@mail.com",

                    Box2Icon = "bi-github",
                    Box2Title = "GitHub",
                    Box2Text = "github.com/mertcan",
                    Box2Url = "https://github.com/mertcan",

                    Box3Icon = "bi-linkedin",
                    Box3Title = "LinkedIn",
                    Box3Text = "linkedin.com/in/mertcan",
                    Box3Url = "https://linkedin.com/in/mertcan",

                    UpdatedDate = DateTime.Now
                };

                db.ContactSectionSettings.Add(setting);
                db.SaveChanges();
            }

            return View(setting);
        }

        /// <summary>
        /// Updates the contact section settings with the submitted form data.
        /// All string values are trimmed before being saved.
        /// Empty or whitespace-only inputs are stored as null.
        /// </summary>
        /// <param name="model">Posted contact section settings model.</param>
        /// <returns>Redirects back to the Edit page after saving changes.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContactSectionSettings model)
        {
            var auth = CheckAdmin();
            if (auth != null) return auth;

            // Retrieve the existing contact section settings record.
            var setting = db.ContactSectionSettings.FirstOrDefault();
            if (setting == null)
                return HttpNotFound();

            // Update the main section content.
            setting.Title = string.IsNullOrWhiteSpace(model.Title) ? null : model.Title.Trim();
            setting.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
            setting.BadgeText = string.IsNullOrWhiteSpace(model.BadgeText) ? null : model.BadgeText.Trim();

            // Update contact box 1 fields.
            setting.Box1Icon = string.IsNullOrWhiteSpace(model.Box1Icon) ? null : model.Box1Icon.Trim();
            setting.Box1Title = string.IsNullOrWhiteSpace(model.Box1Title) ? null : model.Box1Title.Trim();
            setting.Box1Text = string.IsNullOrWhiteSpace(model.Box1Text) ? null : model.Box1Text.Trim();
            setting.Box1Url = string.IsNullOrWhiteSpace(model.Box1Url) ? null : model.Box1Url.Trim();

            // Update contact box 2 fields.
            setting.Box2Icon = string.IsNullOrWhiteSpace(model.Box2Icon) ? null : model.Box2Icon.Trim();
            setting.Box2Title = string.IsNullOrWhiteSpace(model.Box2Title) ? null : model.Box2Title.Trim();
            setting.Box2Text = string.IsNullOrWhiteSpace(model.Box2Text) ? null : model.Box2Text.Trim();
            setting.Box2Url = string.IsNullOrWhiteSpace(model.Box2Url) ? null : model.Box2Url.Trim();

            // Update contact box 3 fields.
            setting.Box3Icon = string.IsNullOrWhiteSpace(model.Box3Icon) ? null : model.Box3Icon.Trim();
            setting.Box3Title = string.IsNullOrWhiteSpace(model.Box3Title) ? null : model.Box3Title.Trim();
            setting.Box3Text = string.IsNullOrWhiteSpace(model.Box3Text) ? null : model.Box3Text.Trim();
            setting.Box3Url = string.IsNullOrWhiteSpace(model.Box3Url) ? null : model.Box3Url.Trim();

            // Store the latest update timestamp.
            setting.UpdatedDate = DateTime.Now;

            db.SaveChanges();

            TempData["Success"] = "İletişim alanı güncellendi.";
            return RedirectToAction("Edit");
        }
    }
}