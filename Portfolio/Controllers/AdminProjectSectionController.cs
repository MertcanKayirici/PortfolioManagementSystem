using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Admin controller responsible for managing project sections.
    /// Each section belongs to a specific project and can include text,
    /// images, buttons, and display settings.
    /// </summary>
    public class AdminProjectSectionController : Controller
    {
        // Database context used for project section operations.
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
        /// Displays all sections for a specific project ordered by DisplayOrder.
        /// </summary>
        /// <param name="projectId">Project ID.</param>
        /// <returns>View containing project sections.</returns>
        public ActionResult Index(int projectId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            // Retrieve project information.
            var project = db.Projects.Find(projectId);
            if (project == null)
                return HttpNotFound();

            ViewBag.Project = project;

            // Get project sections sorted by display order.
            var sections = db.ProjectSections
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            return View(sections);
        }

        /// <summary>
        /// Displays the form to create a new project section.
        /// </summary>
        /// <param name="projectId">Project ID.</param>
        /// <returns>Section creation view.</returns>
        public ActionResult Add(int projectId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var project = db.Projects.Find(projectId);
            if (project == null)
                return HttpNotFound();

            ViewBag.Project = project;
            ViewBag.ProjectId = projectId;

            return View();
        }

        /// <summary>
        /// Creates a new project section with optional image upload.
        /// </summary>
        /// <param name="model">Section model submitted from the form.</param>
        /// <param name="imageFile">Optional uploaded image.</param>
        /// <returns>Redirects to section list or returns view on error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ProjectSections model, HttpPostedFileBase imageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            try
            {
                // Normalize and trim input values.
                model.Title = string.IsNullOrWhiteSpace(model.Title) ? "" : model.Title.Trim();
                model.Description = string.IsNullOrWhiteSpace(model.Description) ? "" : model.Description.Trim();
                model.BulletList = string.IsNullOrWhiteSpace(model.BulletList) ? "" : model.BulletList.Trim();
                model.ButtonText = string.IsNullOrWhiteSpace(model.ButtonText) ? null : model.ButtonText.Trim();
                model.ButtonUrl = string.IsNullOrWhiteSpace(model.ButtonUrl) ? null : model.ButtonUrl.Trim();
                model.ImagePosition = string.IsNullOrWhiteSpace(model.ImagePosition) ? "left" : model.ImagePosition.Trim();

                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;

                // Save image if provided.
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    model.ImageUrl = SaveSectionImage(imageFile);
                }

                db.ProjectSections.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index", new { projectId = model.ProjectId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                // Refill view data in case of error.
                ViewBag.ProjectId = model.ProjectId;
                ViewBag.Project = db.Projects.Find(model.ProjectId);

                return View(model);
            }
        }

        /// <summary>
        /// Displays the edit form for an existing project section.
        /// </summary>
        /// <param name="id">Section ID.</param>
        /// <returns>Edit view.</returns>
        public ActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var section = db.ProjectSections.Find(id);
            if (section == null)
                return HttpNotFound();

            ViewBag.Project = db.Projects.Find(section.ProjectId);
            return View(section);
        }

        /// <summary>
        /// Updates an existing project section and optionally replaces its image.
        /// </summary>
        /// <param name="model">Updated section model.</param>
        /// <param name="imageFile">Optional uploaded image.</param>
        /// <returns>Redirects to section list or returns view on error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectSections model, HttpPostedFileBase imageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            try
            {
                var section = db.ProjectSections.Find(model.ProjectSectionId);
                if (section == null)
                    return HttpNotFound();

                // Update and normalize fields.
                section.Title = string.IsNullOrWhiteSpace(model.Title) ? "" : model.Title.Trim();
                section.Description = string.IsNullOrWhiteSpace(model.Description) ? "" : model.Description.Trim();
                section.BulletList = string.IsNullOrWhiteSpace(model.BulletList) ? "" : model.BulletList.Trim();
                section.ButtonText = string.IsNullOrWhiteSpace(model.ButtonText) ? null : model.ButtonText.Trim();
                section.ButtonUrl = string.IsNullOrWhiteSpace(model.ButtonUrl) ? null : model.ButtonUrl.Trim();
                section.ImagePosition = string.IsNullOrWhiteSpace(model.ImagePosition) ? "left" : model.ImagePosition.Trim();

                section.DisplayOrder = model.DisplayOrder;
                section.IsActive = model.IsActive;
                section.UpdatedDate = DateTime.Now;

                // Replace existing image if a new one is uploaded.
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    if (!string.IsNullOrWhiteSpace(section.ImageUrl))
                    {
                        DeleteFile(section.ImageUrl);
                    }

                    section.ImageUrl = SaveSectionImage(imageFile);
                }

                db.SaveChanges();

                return RedirectToAction("Index", new { projectId = section.ProjectId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Project = db.Projects.Find(model.ProjectId);
                return View(model);
            }
        }

        /// <summary>
        /// Deletes a project section and its associated image file.
        /// </summary>
        /// <param name="id">Section ID.</param>
        /// <returns>Redirects back to the section list.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var section = db.ProjectSections.Find(id);
            if (section == null)
                return HttpNotFound();

            var projectId = section.ProjectId;

            // Delete associated image file if exists.
            if (!string.IsNullOrWhiteSpace(section.ImageUrl))
            {
                DeleteFile(section.ImageUrl);
            }

            db.ProjectSections.Remove(section);
            db.SaveChanges();

            return RedirectToAction("Index", new { projectId = projectId });
        }

        /// <summary>
        /// Validates and saves a section image to the server.
        /// </summary>
        /// <param name="imageFile">Uploaded image file.</param>
        /// <returns>Relative path of the saved image.</returns>
        private string SaveSectionImage(HttpPostedFileBase imageFile)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName);

            if (string.IsNullOrWhiteSpace(extension))
                throw new Exception("Görsel uzantısı bulunamadı.");

            extension = extension.ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Sadece jpg, jpeg, png veya webp yükleyebilirsin.");

            if (imageFile.ContentLength > 5 * 1024 * 1024)
                throw new Exception("Görsel maksimum 5MB olabilir.");

            var folderPath = Server.MapPath("~/Content/img/project-sections/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate unique filename to prevent conflicts.
            var fileName = Guid.NewGuid().ToString("N") + extension;
            var fullPath = Path.Combine(folderPath, fileName);

            imageFile.SaveAs(fullPath);

            return "~/Content/img/project-sections/" + fileName;
        }

        /// <summary>
        /// Deletes a file from the server using its virtual path.
        /// Any exception is silently ignored.
        /// </summary>
        /// <param name="fileUrl">Virtual file path.</param>
        private void DeleteFile(string fileUrl)
        {
            try
            {
                var fullPath = Server.MapPath(fileUrl);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch
            {
                // Intentionally ignored to avoid breaking flow.
            }
        }

        /// <summary>
        /// Toggles the active/passive status of a section via AJAX.
        /// </summary>
        /// <param name="id">Section ID.</param>
        /// <returns>JSON result with updated status.</returns>
        [HttpPost]
        public JsonResult Toggle(int id)
        {
            if (Session["Admin"] == null)
                return Json(new { success = false });

            var section = db.ProjectSections.Find(id);

            if (section == null)
                return Json(new { success = false });

            section.IsActive = !section.IsActive;
            db.SaveChanges();

            return Json(new
            {
                success = true,
                status = section.IsActive
            });
        }
    }
}