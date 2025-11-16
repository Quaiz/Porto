using System;
using System.Linq;
using System.Web.Mvc;
using Porto.Models;

namespace Porto.Controllers
{
    public class AdminUserController : AdminBaseController
    {
        private PortoEntities db = new PortoEntities();

        // GET: Admin/User
        public ActionResult Index()
        {
            var users = db.Users.OrderBy(u => u.UserRole).ThenBy(u => u.Username).ToList();
            return View(users);
        }

        // GET: Admin/User/Create
        public ActionResult Create()
        {
            ViewBag.UserRoles = new SelectList(new[] { "Admin", "Customer" });
            return View();
        }

        // POST: Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (db.Users.Any(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                    ViewBag.UserRoles = new SelectList(new[] { "Admin", "Customer" });
                    return View(user);
                }

                db.Users.Add(user);
                db.SaveChanges();
                TempData["Success"] = "Đã tạo người dùng mới thành công";
                return RedirectToAction("Index");
            }

            ViewBag.UserRoles = new SelectList(new[] { "Admin", "Customer" });
            return View(user);
        }

        // GET: Admin/User/Edit/username
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserRoles = new SelectList(new[] { "Admin", "Customer" }, user.UserRole);
            return View(user);
        }

        // POST: Admin/User/Edit/username
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.Find(user.Username);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                existingUser.Password = user.Password;
                existingUser.UserRole = user.UserRole;
                db.SaveChanges();

                TempData["Success"] = "Đã cập nhật thông tin người dùng";
                return RedirectToAction("Index");
            }

            ViewBag.UserRoles = new SelectList(new[] { "Admin", "Customer" }, user.UserRole);
            return View(user);
        }

        // GET: Admin/User/Delete/username
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Không cho phép xóa chính mình
            if (user.Username == User.Identity.Name)
            {
                TempData["Error"] = "Không thể xóa tài khoản đang đăng nhập";
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // POST: Admin/User/Delete/username
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Check if user is customer and has related data
            var customer = db.Customers.FirstOrDefault(c => c.Username == user.Username);
            if (customer != null)
            {
                var hasOrders = db.Orders.Any(o => o.CustomerID == customer.CustomerID);
                if (hasOrders)
                {
                    TempData["Error"] = "Không thể xóa người dùng này vì có đơn hàng liên quan";
                    return RedirectToAction("Index");
                }
            }

            db.Users.Remove(user);
            db.SaveChanges();
            TempData["Success"] = "Đã xóa người dùng thành công";
            return RedirectToAction("Index");
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

