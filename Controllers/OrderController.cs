using System;
using System.Linq;
using System.Web.Mvc;
using Porto.Models;

namespace Porto.Controllers
{
    // Removed [Authorize(Roles = "Customer")] - check manually instead
    public class OrderController : Controller
    {
        private PortoEntities db = new PortoEntities();

        public ActionResult Checkout()
        {
            // Check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            // Get username from authentication or session
            string username = User.Identity.Name;
            if (string.IsNullOrEmpty(username) && Session["Username"] != null)
            {
                username = Session["Username"].ToString();
            }

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            // Get user and check role
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.UserRole != "Customer")
            {
                TempData["Error"] = "Chỉ khách hàng mới có thể thanh toán";
                return RedirectToAction("Index", "Home");
            }

            var cart = CartSession.GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction("Index", "Cart");
            }

            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            ViewBag.Customer = customer;
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string shippingAddress, string phone, string paymentMethod)
        {
            // Check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            // Get username from authentication
            string username = User.Identity.Name;
            if (string.IsNullOrEmpty(username) && Session["Username"] != null)
            {
                username = Session["Username"].ToString();
            }

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Order") });
            }

            var cart = CartSession.GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction("Index", "Cart");
            }

            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            // Xác định trạng thái thanh toán
            string paymentStatus = "Pending";
            if (paymentMethod == "COD")
            {
                paymentStatus = "COD - Chưa thanh toán";
            }
            else if (paymentMethod == "Online")
            {
                paymentStatus = "Online - Đã thanh toán";
            }
            else if (paymentMethod == "Bank Transfer")
            {
                paymentStatus = "Chuyển khoản - Chờ xác nhận";
            }

            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = CartSession.GetTotal(),
                PaymentStatus = paymentStatus,
                AddressDelivery = shippingAddress
            };

            db.Orders.Add(order);
            db.SaveChanges();

            foreach (var item in cart)
            {
                var detail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                };
                db.OrderDetails.Add(detail);
            }

            db.SaveChanges();
            CartSession.ClearCart();

            // Lưu payment method vào TempData để hiển thị ở trang success
            TempData["PaymentMethod"] = paymentMethod;

            return RedirectToAction("OrderSuccess", new { id = order.OrderID });
        }

        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        public ActionResult History()
        {
            // Check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("History", "Order") });
            }

            string username = User.Identity.Name;
            if (string.IsNullOrEmpty(username) && Session["Username"] != null)
            {
                username = Session["Username"].ToString();
            }

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            var orders = db.Orders.Where(o => o.CustomerID == customer.CustomerID).OrderByDescending(o => o.OrderDate).ToList();
            return View(orders);
        }

        public ActionResult OrderDetails(int id)
        {
            // Check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("OrderDetails", "Order", new { id = id }) });
            }

            string username = User.Identity.Name;
            if (string.IsNullOrEmpty(username) && Session["Username"] != null)
            {
                username = Session["Username"].ToString();
            }

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            var order = db.Orders.Include("OrderDetails").Include("OrderDetails.Product").FirstOrDefault(o => o.OrderID == id && o.CustomerID == customer.CustomerID);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
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