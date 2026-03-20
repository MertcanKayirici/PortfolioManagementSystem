using System;
using System.Linq;
using System.Web.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    public class AdminLoginLogController : Controller
    {
        // Entity Framework database context
        PortfolioDbEntities db = new PortfolioDbEntities();

        // Checks whether the current session belongs to an authenticated admin user
        private bool IsAdmin()
        {
            return Session["Admin"] != null;
        }

        // Displays login logs with optional filtering by username, status, and date range
        public ActionResult Index(string keyword, string status, string startDate, string endDate)
        {
            // Redirect unauthorized users to the login page
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            // Start with all login logs as a queryable collection
            var logs = db.LoginLogs.AsQueryable();

            // Filter by username keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                logs = logs.Where(x => x.Username != null && x.Username.Contains(keyword));
            }

            // Filter by login result status
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "success")
                    logs = logs.Where(x => x.IsSuccess == true);
                else if (status == "fail")
                    logs = logs.Where(x => x.IsSuccess == false);
            }

            // Filter by start date
            if (!string.IsNullOrWhiteSpace(startDate))
            {
                DateTime parsedStartDate;
                if (DateTime.TryParse(startDate, out parsedStartDate))
                {
                    logs = logs.Where(x => x.CreatedAt >= parsedStartDate);
                }
            }

            // Filter by end date
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                DateTime parsedEndDate;
                if (DateTime.TryParse(endDate, out parsedEndDate))
                {
                    // Include the full selected end date by moving to the next day
                    parsedEndDate = parsedEndDate.Date.AddDays(1);
                    logs = logs.Where(x => x.CreatedAt < parsedEndDate);
                }
            }

            // Sort logs from newest to oldest
            logs = logs.OrderByDescending(x => x.CreatedAt);

            // Preserve filter values for the view
            ViewBag.Keyword = keyword;
            ViewBag.Status = status;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // Return filtered results to the view
            return View(logs.ToList());
        }
    }
}