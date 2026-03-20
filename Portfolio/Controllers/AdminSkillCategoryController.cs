using System.Linq;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Admin controller responsible for managing skill categories.
    /// Provides listing, filtering, sorting, create, update, delete,
    /// and AJAX-based active/passive toggle operations.
    /// </summary>
    public class AdminSkillCategoryController : Controller
    {
        // Database context used for skill category and related skill operations.
        PortfolioDbEntities db = new PortfolioDbEntities();

        /// <summary>
        /// Displays the list of skill categories with optional filtering and sorting.
        /// Supports keyword search, active/passive status filtering, color filtering,
        /// and multiple sorting options.
        /// </summary>
        /// <param name="keyword">Search keyword for CategoryName or DisplayName.</param>
        /// <param name="status">Status filter ("active" or "passive").</param>
        /// <param name="color">Hex color filter.</param>
        /// <param name="sort">Sorting option.</param>
        /// <returns>View containing filtered and sorted category list.</returns>
        public ActionResult Index(string keyword, string status, string color, string sort = "order_asc")
        {
            // Prevent unauthorized access to the admin panel.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            // Initialize queryable category collection.
            var categories = db.SkillCategories.AsQueryable();

            // Apply keyword-based search on category name and display name.
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                categories = categories.Where(x =>
                    x.CategoryName.Contains(keyword) ||
                    x.DisplayName.Contains(keyword));
            }

            // Apply active/passive status filtering.
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "active")
                {
                    categories = categories.Where(x => x.IsActive == true);
                }
                else if (status == "passive")
                {
                    categories = categories.Where(x => x.IsActive == false);
                }
            }

            // Apply exact color-based filtering.
            if (!string.IsNullOrWhiteSpace(color))
            {
                color = color.Trim().ToLower();
                categories = categories.Where(x => x.HexColor != null && x.HexColor.ToLower() == color);
            }

            // Apply selected sorting option.
            switch (sort)
            {
                case "order_desc":
                    categories = categories.OrderByDescending(x => x.DisplayOrder);
                    break;

                case "name_asc":
                    categories = categories.OrderBy(x => x.DisplayName);
                    break;

                case "name_desc":
                    categories = categories.OrderByDescending(x => x.DisplayName);
                    break;

                default:
                    categories = categories.OrderBy(x => x.DisplayOrder);
                    break;
            }

            var result = categories.ToList();

            // Preserve filter and sorting values for the UI.
            ViewBag.Keyword = keyword;
            ViewBag.Status = status;
            ViewBag.Color = color;
            ViewBag.Sort = sort;

            // Summary counts for dashboard-style display.
            ViewBag.ActiveCount = db.SkillCategories.Count(x => x.IsActive == true);
            ViewBag.PassiveCount = db.SkillCategories.Count(x => x.IsActive == false);
            ViewBag.TotalCount = db.SkillCategories.Count();

            // Populate distinct color list for filter dropdown or selection UI.
            ViewBag.ColorList = db.SkillCategories
                .Where(x => x.HexColor != null && x.HexColor != "")
                .Select(x => x.HexColor)
                .Distinct()
                .ToList();

            return View(result);
        }

        /// <summary>
        /// Toggles the active/passive status of a skill category via AJAX.
        /// </summary>
        /// <param name="id">Skill category ID.</param>
        /// <returns>JSON result indicating success and updated status.</returns>
        [HttpPost]
        public JsonResult Toggle(int id)
        {
            // Find the selected category record.
            var item = db.SkillCategories.Find(id);

            if (item == null)
                return Json(new { success = false });

            // Reverse the current active status.
            item.IsActive = !item.IsActive;
            db.SaveChanges();

            return Json(new { success = true, status = item.IsActive });
        }

        /// <summary>
        /// Displays the form for creating a new skill category.
        /// </summary>
        /// <returns>Category creation view.</returns>
        public ActionResult Add()
        {
            // Prevent unauthorized access to the admin panel.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        /// <summary>
        /// Creates a new skill category record.
        /// Input values are trimmed before saving.
        /// </summary>
        /// <param name="category">Skill category model submitted from the form.</param>
        /// <returns>Redirects to category list after successful creation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(SkillCategories category)
        {
            // Prevent unauthorized access to the admin panel.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            if (category == null)
                return View();

            // Normalize string values before persistence.
            category.CategoryName = category.CategoryName != null ? category.CategoryName.Trim() : "";
            category.DisplayName = category.DisplayName != null ? category.DisplayName.Trim() : "";
            category.HexColor = category.HexColor != null ? category.HexColor.Trim() : "";

            db.SkillCategories.Add(category);
            db.SaveChanges();

            TempData["Success"] = "Kategori eklendi.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the edit form for an existing skill category.
        /// </summary>
        /// <param name="id">Skill category ID.</param>
        /// <returns>Edit view for the selected category.</returns>
        public ActionResult Edit(int id)
        {
            // Prevent unauthorized access to the admin panel.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            var category = db.SkillCategories.Find(id);

            if (category == null)
                return HttpNotFound();

            return View(category);
        }

        /// <summary>
        /// Updates an existing skill category record.
        /// String values are trimmed before saving.
        /// </summary>
        /// <param name="model">Updated skill category model.</param>
        /// <returns>Redirects to category list after successful update.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SkillCategories model)
        {
            // Prevent unauthorized access to the admin panel.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            var category = db.SkillCategories.Find(model.SkillCategoryId);

            if (category == null)
                return HttpNotFound("Düzenlenecek veri bulunamadı.");

            // Update editable fields with trimmed values.
            category.CategoryName = model.CategoryName != null ? model.CategoryName.Trim() : "";
            category.DisplayName = model.DisplayName != null ? model.DisplayName.Trim() : "";
            category.HexColor = model.HexColor != null ? model.HexColor.Trim() : "";
            category.DisplayOrder = model.DisplayOrder;
            category.IsActive = model.IsActive;

            db.SaveChanges();

            TempData["Updated"] = "Kategori güncellendi.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Deletes a skill category and all related skills associated with it.
        /// Related skills are identified by matching the category name.
        /// </summary>
        /// <param name="id">Skill category ID.</param>
        /// <returns>Redirects to category list after deletion.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            // Prevent unauthorized access to the admin panel.
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            var category = db.SkillCategories.Find(id);

            if (category != null)
            {
                // Retrieve all skills linked to this category.
                var relatedSkills = db.Skills
                    .Where(x => x.Category == category.CategoryName)
                    .ToList();

                // Remove related skills first to maintain data consistency.
                foreach (var skill in relatedSkills)
                {
                    db.Skills.Remove(skill);
                }

                db.SkillCategories.Remove(category);
                db.SaveChanges();
            }

            TempData["Deleted"] = "Kategori ve bağlı skilller silindi.";
            return RedirectToAction("Index");
        }
    }
}