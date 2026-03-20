using System;
using System.Linq;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    public class AdminMessageController : Controller
    {
        PortfolioDbEntities db = new PortfolioDbEntities();

        public ActionResult Index(string subject, string readStatus, string startDate, string endDate)
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            var messages = db.Messages.AsQueryable();

            // Konu filtresi
            if (!string.IsNullOrWhiteSpace(subject))
            {
                messages = messages.Where(x => x.Subject == subject);
            }

            // Okundu / Okunmadı filtresi
            if (!string.IsNullOrWhiteSpace(readStatus))
            {
                if (readStatus == "read")
                    messages = messages.Where(x => x.IsRead == true);
                else if (readStatus == "unread")
                    messages = messages.Where(x => x.IsRead == false);
            }

            // Başlangıç tarihi filtresi
            if (!string.IsNullOrWhiteSpace(startDate))
            {
                DateTime parsedStartDate;
                if (DateTime.TryParse(startDate, out parsedStartDate))
                {
                    messages = messages.Where(x => x.SendDate >= parsedStartDate);
                }
            }

            // Bitiş tarihi filtresi
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                DateTime parsedEndDate;
                if (DateTime.TryParse(endDate, out parsedEndDate))
                {
                    // Günün tamamını kapsasın diye +1 gün yapıp küçüktür ile filtreliyoruz
                    parsedEndDate = parsedEndDate.Date.AddDays(1);
                    messages = messages.Where(x => x.SendDate < parsedEndDate);
                }
            }

            // Sıralama
            messages = messages
                .OrderBy(x => x.IsRead)
                .ThenByDescending(x => x.SendDate);

            // ViewBag değerleri
            ViewBag.Subject = subject;
            ViewBag.ReadStatus = readStatus;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // Dropdown için konu listesi
            ViewBag.SubjectList = db.Messages
                .Where(x => !string.IsNullOrEmpty(x.Subject))
                .Select(x => x.Subject)
                .Distinct()
                .ToList();

            return View(messages.ToList());
        }

        public ActionResult Detail(int id)
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            var message = db.Messages.Find(id);

            if (message == null)
                return HttpNotFound();

            if (!message.IsRead)
            {
                message.IsRead = true;
                db.SaveChanges();
            }

            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login", "Auth");

            var message = db.Messages.Find(id);

            if (message == null)
                return HttpNotFound();

            db.Messages.Remove(message);
            db.SaveChanges();

            TempData["Deleted"] = "Mesaj başarıyla silindi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ToggleRead(int id)
        {
            if (Session["Admin"] == null)
                return Json(new { success = false });

            var message = db.Messages.Find(id);

            if (message == null)
                return Json(new { success = false });

            message.IsRead = !message.IsRead;
            db.SaveChanges();

            return Json(new
            {
                success = true,
                status = message.IsRead
            });
        }
    }
}