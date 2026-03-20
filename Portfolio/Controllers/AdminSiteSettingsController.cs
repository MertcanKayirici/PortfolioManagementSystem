using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    public class AdminSiteSettingsController : Controller
    {
        PortfolioDbEntities db = new PortfolioDbEntities();

        private bool IsAdmin()
        {
            return Session["Admin"] != null;
        }

        public ActionResult Edit()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var site = db.SiteSettings.FirstOrDefault();

            if (site == null)
            {
                site = new SiteSettings
                {
                    SiteTitle = "Portfolio",
                    HeroTitle = "Merhaba, Ben Mertcan",
                    HeroSubtitle = "Backend Developer",
                    HeroBackgroundImageUrl = "~/Content/img/default-hero.jpg",
                    FooterText = "© 2026 Mertcan Kayırıcı"
                };

                db.SiteSettings.Add(site);
                db.SaveChanges();
            }

            return View(site);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SiteSettings model, HttpPostedFileBase heroImageFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var site = db.SiteSettings.FirstOrDefault();
            if (site == null) return HttpNotFound();

            site.SiteTitle = model.SiteTitle != null ? model.SiteTitle.Trim() : "";
            site.HeroTitle = model.HeroTitle != null ? model.HeroTitle.Trim() : "";
            site.HeroSubtitle = model.HeroSubtitle != null ? model.HeroSubtitle.Trim() : "";
            site.FooterText = model.FooterText != null ? model.FooterText.Trim() : "";

            if (heroImageFile != null && heroImageFile.ContentLength > 0)
            {
                if (!string.IsNullOrEmpty(site.HeroBackgroundImageUrl) &&
                    !site.HeroBackgroundImageUrl.Contains("default-hero.jpg"))
                {
                    DeleteImage(site.HeroBackgroundImageUrl);
                }

                site.HeroBackgroundImageUrl = SaveHeroImage(heroImageFile);
            }

            db.SaveChanges();

            TempData["Updated"] = "Site ayarları güncellendi.";
            return RedirectToAction("Edit");
        }

        private string SaveHeroImage(HttpPostedFileBase imageFile)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName);

            if (string.IsNullOrWhiteSpace(extension))
                throw new Exception("Dosya uzantısı bulunamadı.");

            extension = extension.ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Sadece jpg, jpeg, png veya webp yükleyebilirsin.");

            if (imageFile.ContentLength > 10 * 1024 * 1024)
                throw new Exception("Hero resmi maksimum 5MB olmalı.");

            var folderPath = Server.MapPath("~/Content/img/site/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString("N") + extension;
            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                imageFile.InputStream.CopyTo(stream);
            }

            return "~/Content/img/site/" + fileName;
        }

        private void DeleteImage(string imageUrl)
        {
            try
            {
                var fullPath = Server.MapPath(imageUrl);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch
            {
            }
        }
    }
}