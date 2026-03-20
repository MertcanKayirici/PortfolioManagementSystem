using Portfolio.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Admin controller responsible for managing portfolio projects.
    /// Includes listing, filtering, sorting, creating, updating, deleting,
    /// image upload, and active/passive toggle operations.
    /// </summary>
    public class AdminProjectController : Controller
    {
        // Database context used for project-related operations.
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
        /// Displays the project list with optional search, status filtering,
        /// and sorting options.
        /// </summary>
        /// <param name="keyword">Search keyword for title or short description.</param>
        /// <param name="status">Project status filter ("active" or "passive").</param>
        /// <param name="sort">Sorting option for project ordering.</param>
        /// <returns>View containing the filtered and sorted project list.</returns>
        public ActionResult Index(string keyword, string status, string sort = "order_asc")
        {
            var auth = CheckAdmin();
            if (auth != null) return auth;

            // Initialize queryable project collection.
            var projects = db.Projects.AsQueryable();

            // Apply keyword-based search on title and short description.
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                projects = projects.Where(x =>
                    (x.Title != null && x.Title.Contains(keyword)) ||
                    (x.ShortDescription != null && x.ShortDescription.Contains(keyword)));
            }

            // Apply project status filter.
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "active")
                    projects = projects.Where(x => x.IsActive == true);
                else if (status == "passive")
                    projects = projects.Where(x => x.IsActive == false);
            }

            // Apply sorting based on selected option.
            switch (sort)
            {
                case "order_desc":
                    projects = projects.OrderByDescending(x => x.DisplayOrder);
                    break;

                case "name_asc":
                    projects = projects.OrderBy(x => x.Title);
                    break;

                case "name_desc":
                    projects = projects.OrderByDescending(x => x.Title);
                    break;

                default:
                    projects = projects.OrderBy(x => x.DisplayOrder);
                    break;
            }

            // Dashboard-style summary information for the view.
            ViewBag.TotalCount = db.Projects.Count();
            ViewBag.ActiveCount = db.Projects.Count(x => x.IsActive);
            ViewBag.PassiveCount = db.Projects.Count(x => !x.IsActive);

            // Preserve current filter and sort values in the UI.
            ViewBag.Keyword = keyword;
            ViewBag.Status = status;
            ViewBag.Sort = sort;

            return View(projects.ToList());
        }

        /// <summary>
        /// Displays the project creation form.
        /// </summary>
        /// <returns>Project add view.</returns>
        public ActionResult Add()
        {
            var auth = CheckAdmin();
            if (auth != null) return auth;

            return View();
        }

        /// <summary>
        /// Creates a new project record with optional image upload support.
        /// If no image is uploaded, a default project thumbnail is assigned.
        /// </summary>
        /// <param name="project">Project model submitted from the form.</param>
        /// <param name="imageFile">Optional uploaded project image.</param>
        /// <returns>Redirects to the project list if successful; otherwise returns the same view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Projects project, HttpPostedFileBase imageFile)
        {
            var auth = CheckAdmin();
            if (auth != null) return auth;

            try
            {
                if (!ModelState.IsValid)
                    return View(project);

                // Normalize and trim project input values.
                CleanProject(project);

                project.CreatedDate = DateTime.Now;
                project.ThumbnailUrl = "~/Content/img/default-project.jpg";

                // Save uploaded image if provided.
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    project.ThumbnailUrl = SaveProjectImage(imageFile);
                }

                db.Projects.Add(project);
                db.SaveChanges();

                TempData["Success"] = "Proje eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return HandleError(ex, project);
            }
        }

        /// <summary>
        /// Displays the edit form for an existing project.
        /// </summary>
        /// <param name="id">Project ID.</param>
        /// <returns>Edit view for the selected project.</returns>
        public ActionResult Edit(int id)
        {
            var auth = CheckAdmin();
            if (auth != null) return auth;

            var project = db.Projects.Find(id);
            if (project == null) return HttpNotFound();

            return View(project);
        }

        /// <summary>
        /// Updates an existing project and optionally replaces its thumbnail image.
        /// </summary>
        /// <param name="project">Updated project model submitted from the form.</param>
        /// <param name="imageFile">Optional uploaded project image.</param>
        /// <returns>Redirects to the project list if successful; otherwise returns the same view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Projects project, HttpPostedFileBase imageFile)
        {
            var auth = CheckAdmin();
            if (auth != null) return auth;

            try
            {
                if (!ModelState.IsValid)
                    return View(project);

                // Retrieve the existing project from the database.
                var existingProject = db.Projects.Find(project.ProjectId);
                if (existingProject == null)
                    return HttpNotFound();

                // Normalize and trim incoming project data.
                CleanProject(project);

                // Update editable project fields.
                existingProject.Title = project.Title;
                existingProject.ShortDescription = project.ShortDescription;
                existingProject.Description = project.Description;
                existingProject.Technologies = project.Technologies;
                existingProject.GithubUrl = project.GithubUrl;
                existingProject.LiveDemoUrl = project.LiveDemoUrl;
                existingProject.DisplayOrder = project.DisplayOrder;
                existingProject.IsFeatured = project.IsFeatured;
                existingProject.IsActive = project.IsActive;

                // Update modification timestamp.
                // Remove this line if the Projects table does not include an UpdatedDate column.
                existingProject.UpdatedDate = DateTime.Now;

                // Replace existing thumbnail image if a new one is uploaded.
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    if (!string.IsNullOrWhiteSpace(existingProject.ThumbnailUrl) &&
                        !existingProject.ThumbnailUrl.Contains("default-project.jpg"))
                    {
                        DeleteProjectImage(existingProject.ThumbnailUrl);
                    }

                    existingProject.ThumbnailUrl = SaveProjectImage(imageFile);
                }

                db.SaveChanges();

                TempData["Success"] = "Proje güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return HandleError(ex, project);
            }
        }

        /// <summary>
        /// Deletes a project and removes its thumbnail image from the server
        /// if the image is not the default placeholder.
        /// </summary>
        /// <param name="id">Project ID.</param>
        /// <returns>Redirects to the project list after deletion.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var auth = CheckAdmin();
            if (auth != null) return auth;

            try
            {
                var project = db.Projects.Find(id);
                if (project == null) return HttpNotFound();

                // Delete custom thumbnail image if it exists.
                if (!string.IsNullOrEmpty(project.ThumbnailUrl) &&
                    !project.ThumbnailUrl.Contains("default-project.jpg"))
                {
                    DeleteProjectImage(project.ThumbnailUrl);
                }

                db.Projects.Remove(project);
                db.SaveChanges();

                TempData["Deleted"] = "Proje silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Cleans and trims project string properties before database persistence.
        /// Empty or whitespace-only values are converted to null.
        /// </summary>
        /// <param name="project">Project entity to be normalized.</param>
        private void CleanProject(Projects project)
        {
            project.Title = string.IsNullOrWhiteSpace(project.Title)
                ? null
                : project.Title.Trim();

            project.ShortDescription = string.IsNullOrWhiteSpace(project.ShortDescription)
                ? null
                : project.ShortDescription.Trim();

            project.Description = string.IsNullOrWhiteSpace(project.Description)
                ? null
                : project.Description.Trim();

            project.Technologies = string.IsNullOrWhiteSpace(project.Technologies)
                ? null
                : project.Technologies.Trim();

            project.GithubUrl = string.IsNullOrWhiteSpace(project.GithubUrl)
                ? null
                : project.GithubUrl.Trim();

            project.LiveDemoUrl = string.IsNullOrWhiteSpace(project.LiveDemoUrl)
                ? null
                : project.LiveDemoUrl.Trim();
        }

        /// <summary>
        /// Handles exceptions raised during add/edit operations and prepares
        /// user-friendly error messages for the UI.
        /// </summary>
        /// <param name="ex">Thrown exception.</param>
        /// <param name="project">Project model to return back to the view.</param>
        /// <returns>View containing the project model and error message.</returns>
        private ActionResult HandleError(Exception ex, Projects project)
        {
            if (ex is DbEntityValidationException validationEx)
            {
                var sb = new StringBuilder();

                foreach (var eve in validationEx.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(ve.PropertyName + ": " + ve.ErrorMessage);
                    }
                }

                TempData["Error"] = sb.ToString();
            }
            else if (ex is DbUpdateException updateEx)
            {
                TempData["Error"] = updateEx.InnerException?.InnerException?.Message ?? updateEx.Message;
            }
            else
            {
                TempData["Error"] = ex.Message;
            }

            return View(project);
        }

        /// <summary>
        /// Validates and saves an uploaded project image to the server.
        /// Accepts jpg, jpeg, png, and webp formats up to 5 MB.
        /// </summary>
        /// <param name="imageFile">Uploaded image file.</param>
        /// <returns>Relative virtual path of the saved image.</returns>
        /// <exception cref="Exception">
        /// Thrown when the file is missing, invalid, too large, or cannot be saved.
        /// </exception>
        private string SaveProjectImage(HttpPostedFileBase imageFile)
        {
            if (imageFile == null || imageFile.ContentLength <= 0)
                throw new Exception("Lütfen bir resim seç.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName);

            if (string.IsNullOrWhiteSpace(extension))
                throw new Exception("Dosya uzantısı bulunamadı.");

            extension = extension.ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Sadece jpg, jpeg, png veya webp yükleyebilirsin.");

            if (imageFile.ContentLength > 5 * 1024 * 1024)
                throw new Exception("Resim maksimum 5MB olmalı.");

            var folderPath = Server.MapPath("~/Content/img/projects/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate a unique file name to avoid collisions.
            var safeFileName = Guid.NewGuid().ToString("N") + extension;
            var fullPath = Path.Combine(folderPath, safeFileName);

            try
            {
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    imageFile.InputStream.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Resim kaydedilirken hata oluştu: " + ex.Message);
            }

            return "~/Content/img/projects/" + safeFileName;
        }

        /// <summary>
        /// Deletes a project thumbnail image from the server using its virtual path.
        /// Any exception is silently ignored to avoid interrupting the request flow.
        /// </summary>
        /// <param name="imageUrl">Virtual path of the image to delete.</param>
        private void DeleteProjectImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                    return;

                var fullPath = Server.MapPath(imageUrl);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch
            {
                // Intentionally ignored:
                // image deletion failure should not break the application flow.
            }
        }

        /// <summary>
        /// Toggles the active/passive status of a project via AJAX.
        /// </summary>
        /// <param name="id">Project ID.</param>
        /// <returns>JSON result indicating success and updated status.</returns>
        [HttpPost]
        public JsonResult Toggle(int id)
        {
            if (Session["Admin"] == null)
                return Json(new { success = false });

            var project = db.Projects.Find(id);

            if (project == null)
                return Json(new { success = false });

            project.IsActive = !project.IsActive;
            db.SaveChanges();

            return Json(new
            {
                success = true,
                status = project.IsActive
            });
        }
    }
}