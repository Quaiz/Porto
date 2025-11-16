using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Porto.Models;

namespace Porto.Controllers
{
    public class AccountController : Controller
    {
        private PortoEntities db = new PortoEntities();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // WARNING: Plain text password comparison - In production, use hashed passwords
                // Consider using: ASP.NET Identity, BCrypt, or System.Security.Cryptography
                var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(username, false);
                    Session["UserRole"] = user.UserRole;
                    Session["Username"] = user.Username;

                    // Redirect theo role
                    if (user.UserRole == "Admin")
                    {
                        // Admin luôn vào Admin Panel
                        return RedirectToAction("Index", "AdminProduct");
                    }
                    else
                    {
                        // Customer về trang chủ hoặc returnUrl
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user, string name, string email, string phone, string address)
        {
            if (ModelState.IsValid)
            {
                var existing = db.Users.Any(u => u.Username == user.Username);
                if (existing)
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View(user);
                }

                // WARNING: Storing plain text password - In production, hash the password
                // Example: user.Password = HashPassword(user.Password);
                user.UserRole = "Customer";
                db.Users.Add(user);
                db.SaveChanges();

                var customer = new Customer
                {
                    CustomerName = name,
                    CustomerEmail = email,
                    CustomerPhone = phone,
                    CustomerAddress = address,
                    Username = user.Username
                };
                db.Customers.Add(customer);
                db.SaveChanges();

                FormsAuthentication.SetAuthCookie(user.Username, false);
                Session["UserRole"] = "Customer";
                Session["Username"] = user.Username;

                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}