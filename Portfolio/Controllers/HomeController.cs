using System.Linq;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Public-facing home controller responsible for loading
    /// the main portfolio page data.
    /// Aggregates about, skills, categories, projects,
    /// site settings, social media links, and contact section content.
    /// </summary>
    public class HomeController : Controller
    {
        // Database context used for retrieving homepage content.
        PortfolioDbEntities db = new PortfolioDbEntities();

        /// <summary>
        /// Loads and displays the main homepage of the portfolio application.
        /// Retrieves active and ordered content from multiple database tables
        /// and passes them to the view via ViewBag.
        /// </summary>
        /// <returns>Homepage view with all required portfolio data.</returns>
        public ActionResult Index()
        {
            // Retrieve the first available About section record.
            var about = db.Abouts.FirstOrDefault();

            // Retrieve active skills ordered by display priority.
            var skills = db.Skills
                           .Where(x => x.IsActive == true)
                           .OrderBy(x => x.DisplayOrder)
                           .ToList();

            // Retrieve active skill categories ordered by display priority.
            var skillCategories = db.SkillCategories
                            .Where(x => x.IsActive == true)
                            .OrderBy(x => x.DisplayOrder)
                            .ToList();

            ViewBag.SkillCategories = skillCategories;

            // Retrieve active projects ordered by display priority.
            var projects = db.Projects
                             .Where(x => x.IsActive == true)
                             .OrderBy(x => x.DisplayOrder)
                             .ToList();

            // Retrieve global site settings.
            var siteSetting = db.SiteSettings.FirstOrDefault();

            // Retrieve active social media links ordered by display priority.
            var socialMedias = db.SocialMedias
                                 .Where(x => x.IsActive == true)
                                 .OrderBy(x => x.DisplayOrder)
                                 .ToList();

            // Pass retrieved content to the view.
            ViewBag.About = about;
            ViewBag.Skills = skills;
            ViewBag.Projects = projects;
            ViewBag.SiteSetting = siteSetting;
            ViewBag.SocialMedias = socialMedias;
            ViewBag.ContactSection = db.ContactSectionSettings.FirstOrDefault();

            return View();
        }
    }
}