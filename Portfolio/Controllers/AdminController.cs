using System.Linq;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Main admin dashboard controller.
    /// Provides summary statistics for the admin panel such as
    /// skills, projects, and messages.
    /// </summary>
    public class AdminController : Controller
    {
        // Database context used to retrieve dashboard data.
        PortfolioDbEntities db = new PortfolioDbEntities();

        /// <summary>
        /// Displays the admin dashboard.
        /// If the user is not authenticated as an admin, redirects to the login page.
        /// </summary>
        /// <returns>Dashboard view with summary data passed via ViewBag.</returns>
        public ActionResult Index()
        {
            // Prevent unauthorized access to the admin panel.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            // Total number of skills.
            ViewBag.SkillCount = db.Skills.Count();

            // Total number of projects.
            ViewBag.ProjectCount = db.Projects.Count();

            // Total number of messages.
            ViewBag.MessageCount = db.Messages.Count();

            // Number of active skills.
            ViewBag.ActiveSkill = db.Skills.Count(x => x.IsActive);

            // Number of unread messages.
            ViewBag.UnreadMessageCount = db.Messages.Count(x => x.IsRead == false);

            return View();
        }
    }
}