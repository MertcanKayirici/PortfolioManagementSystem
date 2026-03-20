using Portfolio.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Portfolio.Controllers
{
    /// <summary>
    /// Authentication controller responsible for admin login and logout operations.
    /// Also records login/logout activity logs including IP address information.
    /// </summary>
    public class AuthController : Controller
    {
        /// <summary>
        /// Displays the login page.
        /// If the admin session already exists, redirects to the admin dashboard.
        /// </summary>
        /// <returns>Login view or redirect to admin dashboard.</returns>
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["Admin"] != null)
                return RedirectToAction("Index", "Admin");

            return View();
        }

        /// <summary>
        /// Handles admin login requests.
        /// Validates input values, checks static admin credentials,
        /// creates session values on success, and records login attempts.
        /// </summary>
        /// <param name="username">Submitted username.</param>
        /// <param name="password">Submitted password.</param>
        /// <returns>Redirects to admin dashboard on success; otherwise returns login view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            // Prevent authenticated admins from accessing the login page again.
            if (Session["Admin"] != null)
                return RedirectToAction("Index", "Admin");

            // Normalize input values.
            username = string.IsNullOrWhiteSpace(username) ? "" : username.Trim();
            password = string.IsNullOrWhiteSpace(password) ? "" : password.Trim();

            // Reject empty username or password inputs.
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                AddLoginLog(username, false, "Boş kullanıcı adı veya şifre");
                ViewBag.Error = "Kullanıcı adı ve şifre boş bırakılamaz.";
                return View();
            }

            // Validate static admin credentials.
            if (username == "admin" && password == "123456")
            {
                Session["Admin"] = true;
                Session["AdminUsername"] = username;
                Session.Timeout = 30;

                AddLoginLog(username, true, "Başarılı giriş");

                return RedirectToAction("Index", "Admin");
            }

            // Record failed login attempt.
            AddLoginLog(username, false, "Hatalı kullanıcı adı veya şifre");
            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        /// <summary>
        /// Logs the current admin out of the system.
        /// Clears and abandons the current session, then redirects to the login page.
        /// </summary>
        /// <returns>Redirect to login page.</returns>
        public ActionResult Logout()
        {
            // Retrieve the current admin username for logging purposes.
            var username = Session["AdminUsername"] != null
                ? Session["AdminUsername"].ToString()
                : "Unknown";

            AddLoginLog(username, true, "Çıkış yapıldı");

            // Clear current session data
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            // Expire ASP.NET session cookie
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var cookie = new HttpCookie("ASP.NET_SessionId")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };

                Response.Cookies.Add(cookie);
            }

            // Prevent browser from serving cached admin pages
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");

            return RedirectToAction("Login", "Auth");
        }

        /// <summary>
        /// Creates a login activity record in the database.
        /// Stores username, success status, IP address, description, and timestamp.
        /// </summary>
        /// <param name="username">Username involved in the login event.</param>
        /// <param name="isSuccess">Indicates whether the login/logout action was successful.</param>
        /// <param name="description">Short description of the event.</param>
        private void AddLoginLog(string username, bool isSuccess, string description)
        {
            try
            {
                using (var db = new PortfolioDbEntities())
                {
                    var log = new LoginLogs
                    {
                        Username = string.IsNullOrWhiteSpace(username) ? null : username.Trim(),
                        IsSuccess = isSuccess,
                        IpAddress = GetUserIpAddress(),
                        Description = description,
                        CreatedAt = DateTime.Now
                    };

                    db.LoginLogs.Add(log);
                    db.SaveChanges();
                }
            }
            catch
            {
                // Intentionally ignored:
                // logging failures should not interrupt the authentication flow.
            }
        }

        /// <summary>
        /// Retrieves the client's IP address.
        /// First checks proxy-forwarded headers, then falls back to the remote address.
        /// </summary>
        /// <returns>User IP address if available; otherwise "Unknown".</returns>
        private string GetUserIpAddress()
        {
            try
            {
                var forwardedIp = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrWhiteSpace(forwardedIp))
                    return forwardedIp.Split(',')[0].Trim();

                var remoteIp = Request.ServerVariables["REMOTE_ADDR"];
                return string.IsNullOrWhiteSpace(remoteIp) ? "Unknown" : remoteIp;
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}