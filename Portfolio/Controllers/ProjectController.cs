using System.Linq;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Public controller responsible for displaying detailed project pages.
    /// Retrieves a specific project along with its active sections.
    /// </summary>
    public class ProjectController : Controller
    {
        // Database context used for project and section retrieval.
        PortfolioDbEntities db = new PortfolioDbEntities();

        /// <summary>
        /// Displays the detail page for a specific project.
        /// Loads the project and its related active sections ordered by display order.
        /// </summary>
        /// <param name="id">Project ID.</param>
        /// <returns>Project detail view with associated sections.</returns>
        public ActionResult Detail(int id)
        {
            // Retrieve project by ID.
            var project = db.Projects.Find(id);
            if (project == null)
                return HttpNotFound();

            // Retrieve active sections related to the project.
            var sections = db.ProjectSections
                .Where(x => x.ProjectId == id && x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            // Pass sections to the view.
            ViewBag.Sections = sections;

            return View(project);
        }
    }
}