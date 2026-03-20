using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Admin controller responsible for managing the "About" section
    /// of the portfolio panel, including profile image and CV upload operations.
    /// </summary>
    public class AdminAboutController : Controller
    {
        // Database context used for About entity operations.
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
        /// Displays the edit page for the About section.
        /// If no About record exists, a default one is created automatically.
        /// </summary>
        /// <returns>Edit view with the existing or newly created About record.</returns>
        public ActionResult Edit()
        {
            // Prevent unauthorized access to the admin panel.
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            // Retrieve the first About record from the database.
            var about = db.Abouts.FirstOrDefault();

            // If no About record exists, create a default entry.
            if (about == null)
            {
                about = new Abouts
                {
                    Title = "Merhaba, Ben Mertcan",
                    Description = "Hakkımda açıklama alanı.",
                    ProfileImageUrl = "~/Content/img/default-profile.jpg",
                    CvUrl = null,
                    UpdatedDate = DateTime.Now
                };

                db.Abouts.Add(about);
                db.SaveChanges();
            }

            return View(about);
        }

        /// <summary>
        /// Updates the About section data, including optional profile image
        /// and CV file upload operations.
        /// </summary>
        /// <param name="model">Posted About model data.</param>
        /// <param name="profileImageFile">Optional uploaded profile image file.</param>
        /// <param name="cvFile">Optional uploaded CV file.</param>
        /// <returns>Redirects back to the Edit page after processing.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Abouts model, HttpPostedFileBase profileImageFile, HttpPostedFileBase cvFile)
        {
            // Prevent unauthorized access to the admin panel.
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            try
            {
                // Find the existing About record by its primary key.
                var about = db.Abouts.Find(model.AboutId);
                if (about == null)
                    return HttpNotFound();

                // Update text fields with trimmed values.
                about.Title = string.IsNullOrWhiteSpace(model.Title) ? "" : model.Title.Trim();
                about.Description = string.IsNullOrWhiteSpace(model.Description) ? "" : model.Description.Trim();
                about.UpdatedDate = DateTime.Now;

                // Check whether a profile image already exists or a new one is uploaded.
                bool hasExistingProfile = !string.IsNullOrWhiteSpace(about.ProfileImageUrl);
                bool hasNewProfile = profileImageFile != null && profileImageFile.ContentLength > 0;

                // Profile image is required if neither an existing nor a new one is available.
                if (!hasExistingProfile && !hasNewProfile)
                {
                    TempData["Error"] = "Profil fotoğrafı zorunludur.";
                    return RedirectToAction("Edit");
                }

                // If a new profile image is uploaded, delete the old one unless it is the default image.
                if (hasNewProfile)
                {
                    if (!string.IsNullOrWhiteSpace(about.ProfileImageUrl) &&
                        !about.ProfileImageUrl.Contains("default-profile.jpg"))
                    {
                        DeleteFile(about.ProfileImageUrl);
                    }

                    // Save the new profile image and update its path.
                    about.ProfileImageUrl = SaveProfileImage(profileImageFile);
                }

                // If a new CV file is uploaded, replace the old one.
                if (cvFile != null && cvFile.ContentLength > 0)
                {
                    if (!string.IsNullOrWhiteSpace(about.CvUrl))
                    {
                        DeleteFile(about.CvUrl);
                    }

                    // Save the new CV file and update its path.
                    about.CvUrl = SaveCvFile(cvFile);
                }

                db.SaveChanges();

                TempData["Updated"] = "Hakkımda alanı güncellendi.";
                return RedirectToAction("Edit");
            }
            catch (Exception ex)
            {
                // Store the exception message for display in the UI.
                TempData["Error"] = ex.Message;
                return RedirectToAction("Edit");
            }
        }

        /// <summary>
        /// Deletes the uploaded CV file asynchronously via AJAX
        /// and clears the stored CV path from the database.
        /// </summary>
        /// <param name="aboutId">The related About record ID.</param>
        /// <returns>JSON result indicating success or failure.</returns>
        [HttpPost]
        public JsonResult DeleteCvAjax(int aboutId)
        {
            var about = db.Abouts.FirstOrDefault(x => x.AboutId == aboutId);
            if (about == null)
                return Json(new { success = false });

            if (!string.IsNullOrEmpty(about.CvUrl))
            {
                DeleteFile(about.CvUrl);
                about.CvUrl = null;
                db.SaveChanges();
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Validates and saves the uploaded profile image to the server.
        /// </summary>
        /// <param name="imageFile">Uploaded profile image file.</param>
        /// <returns>Relative virtual path of the saved image file.</returns>
        /// <exception cref="Exception">
        /// Thrown when file extension is missing, invalid, or file size exceeds the limit.
        /// </exception>
        private string SaveProfileImage(HttpPostedFileBase imageFile)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName);

            if (string.IsNullOrWhiteSpace(extension))
                throw new Exception("Profil fotoğrafı uzantısı bulunamadı.");

            extension = extension.ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Profil fotoğrafı için sadece jpg, jpeg, png veya webp yükleyebilirsin.");

            if (imageFile.ContentLength > 5 * 1024 * 1024)
                throw new Exception("Profil fotoğrafı maksimum 5MB olmalı.");

            var folderPath = Server.MapPath("~/Content/img/profile/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate a unique file name to avoid naming conflicts.
            var fileName = Guid.NewGuid().ToString("N") + extension;
            var fullPath = Path.Combine(folderPath, fileName);

            imageFile.SaveAs(fullPath);

            return "~/Content/img/profile/" + fileName;
        }

        /// <summary>
        /// Validates and saves the uploaded CV file to the server.
        /// </summary>
        /// <param name="cvFile">Uploaded CV file.</param>
        /// <returns>Relative virtual path of the saved CV file.</returns>
        /// <exception cref="Exception">
        /// Thrown when file extension is missing, invalid, or file size exceeds the limit.
        /// </exception>
        private string SaveCvFile(HttpPostedFileBase cvFile)
        {
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var extension = Path.GetExtension(cvFile.FileName);

            if (string.IsNullOrWhiteSpace(extension))
                throw new Exception("CV dosya uzantısı bulunamadı.");

            extension = extension.ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("CV için sadece pdf, doc veya docx yükleyebilirsin.");

            if (cvFile.ContentLength > 10 * 1024 * 1024)
                throw new Exception("CV maksimum 10MB olmalı.");

            var folderPath = Server.MapPath("~/Content/files/cv/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate a unique file name to avoid overwriting existing files.
            var fileName = Guid.NewGuid().ToString("N") + extension;
            var fullPath = Path.Combine(folderPath, fileName);

            cvFile.SaveAs(fullPath);

            return "~/Content/files/cv/" + fileName;
        }

        /// <summary>
        /// Deletes a physical file from the server using its virtual path.
        /// Any exception is silently ignored to avoid breaking the request flow.
        /// </summary>
        /// <param name="fileUrl">Virtual file path to be deleted.</param>
        private void DeleteFile(string fileUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileUrl))
                    return;

                var fullPath = Server.MapPath(fileUrl);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch
            {
                // Intentionally ignored:
                // file deletion failure should not crash the application flow.
            }
        }
    }
}