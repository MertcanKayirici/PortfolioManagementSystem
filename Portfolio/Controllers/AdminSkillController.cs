using System.Linq;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Admin controller responsible for managing skills.
    /// Includes listing, filtering, sorting, creating, updating,
    /// deleting, and AJAX-based status toggling.
    /// </summary>
    public class AdminSkillController : Controller
    {
        // Database context used for skill and category operations.
        PortfolioDbEntities db = new PortfolioDbEntities();

        /// <summary>
        /// Displays the list of skills with optional filtering and sorting options.
        /// Supports keyword search, category filtering, status filtering,
        /// and multiple sorting methods.
        /// </summary>
        /// <param name="keyword">Search keyword for skill name.</param>
        /// <param name="category">Category filter.</param>
        /// <param name="status">Status filter ("active" or "passive").</param>
        /// <param name="sort">Sorting option.</param>
        /// <returns>View containing filtered and sorted skill list.</returns>
        public ActionResult Index(string keyword, string category, string status, string sort = "order_asc")
        {
            // Prevent unauthorized access to the admin panel.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            // Initialize queryable skill collection.
            var skills = db.Skills.AsQueryable();

            // Apply keyword-based search on skill name.
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                skills = skills.Where(x => x.Name.Contains(keyword));
            }

            // Apply category-based filtering.
            if (!string.IsNullOrWhiteSpace(category))
            {
                skills = skills.Where(x => x.Category == category);
            }

            // Apply active/passive status filtering.
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "active")
                {
                    skills = skills.Where(x => x.IsActive == true);
                }
                else if (status == "passive")
                {
                    skills = skills.Where(x => x.IsActive == false);
                }
            }

            // Apply selected sorting option.
            switch (sort)
            {
                case "order_desc":
                    skills = skills.OrderByDescending(x => x.DisplayOrder);
                    break;

                case "name_asc":
                    skills = skills.OrderBy(x => x.Name);
                    break;

                case "name_desc":
                    skills = skills.OrderByDescending(x => x.Name);
                    break;

                case "percent_desc":
                    skills = skills.OrderByDescending(x => x.Percentage);
                    break;

                case "percent_asc":
                    skills = skills.OrderBy(x => x.Percentage);
                    break;

                default:
                    skills = skills.OrderBy(x => x.DisplayOrder);
                    break;
            }

            var result = skills.ToList();

            // Preserve filter and sorting values for UI state.
            ViewBag.Keyword = keyword;
            ViewBag.Category = category;
            ViewBag.Status = status;
            ViewBag.Sort = sort;

            // Summary counts for dashboard display.
            ViewBag.TotalCount = db.Skills.Count();
            ViewBag.ActiveCount = db.Skills.Count(x => x.IsActive == true);
            ViewBag.PassiveCount = db.Skills.Count(x => x.IsActive == false);

            // Load categories for filter dropdown.
            ViewBag.Categories = db.SkillCategories
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            return View(result);
        }

        /// <summary>
        /// Displays the form to create a new skill.
        /// </summary>
        /// <returns>Skill creation view.</returns>
        public ActionResult Add()
        {
            // Prevent unauthorized access.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            // Load categories for selection.
            ViewBag.Categories = db.SkillCategories.ToList();
            return View();
        }

        /// <summary>
        /// Creates a new skill record.
        /// </summary>
        /// <param name="skill">Skill model submitted from the form.</param>
        /// <returns>Redirects to skill list after creation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Skills skill)
        {
            // Prevent unauthorized access.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            db.Skills.Add(skill);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the edit form for an existing skill.
        /// </summary>
        /// <param name="id">Skill ID.</param>
        /// <returns>Edit view for the selected skill.</returns>
        public ActionResult Edit(int id)
        {
            // Prevent unauthorized access.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            var skill = db.Skills.Find(id);

            // Load categories for selection.
            ViewBag.Categories = db.SkillCategories.ToList();

            return View(skill);
        }

        /// <summary>
        /// Updates an existing skill record.
        /// </summary>
        /// <param name="skill">Updated skill model.</param>
        /// <returns>Redirects to skill list after update.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Skills skill)
        {
            // Retrieve existing record from database.
            var existing = db.Skills.Find(skill.SkillId);

            // Update editable fields.
            existing.Name = skill.Name;
            existing.Percentage = skill.Percentage;
            existing.Category = skill.Category;
            existing.DisplayOrder = skill.DisplayOrder;
            existing.IsActive = skill.IsActive;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Deletes a skill from the database.
        /// </summary>
        /// <param name="id">Skill ID.</param>
        /// <returns>Redirects to skill list after deletion.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var skill = db.Skills.Find(id);

            if (skill != null)
            {
                db.Skills.Remove(skill);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Toggles the active/passive status of a skill via AJAX.
        /// </summary>
        /// <param name="id">Skill ID.</param>
        /// <returns>JSON result indicating success and updated status.</returns>
        [HttpPost]
        public JsonResult Toggle(int id)
        {
            var skill = db.Skills.Find(id);

            if (skill != null)
            {
                // Reverse current active status.
                skill.IsActive = !skill.IsActive;
                db.SaveChanges();

                return Json(new { success = true, status = skill.IsActive });
            }

            return Json(new { success = false });
        }
    }
}