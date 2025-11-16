using System;
using System.Linq;
using System.Web.Mvc;
using Porto.Models;

namespace Porto.Controllers
{
    public class AdminCustomerController : AdminBaseController
    {
        private PortoEntities db = new PortoEntities();

        // GET: Admin/Customer
        public ActionResult Index(string search = "")
        {
            var customers = db.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                customers = customers.Where(c => 
                    c.CustomerName.Contains(search) || 
                    c.CustomerEmail.Contains(search) ||
                    c.CustomerPhone.Contains(search) ||
                    c.Username.Contains(search)
                );
            }

            ViewBag.Search = search;
            return View(customers.OrderByDescending(c => c.CustomerID).ToList());
        }

        // GET: Admin/Customer/Details/5
        public ActionResult Details(int id)
        {
            var customer = db.Customers
                .Include("Order")
                .FirstOrDefault(c => c.CustomerID == id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            // Get order statistics
            ViewBag.TotalOrders = customer.Order.Count();
            ViewBag.TotalSpent = customer.Order.Sum(o => (decimal?)o.TotalAmount) ?? 0;
            ViewBag.PendingOrders = customer.Order.Count(o => o.PaymentStatus.Contains("Chưa"));

            return View(customer);
        }

        // GET: Admin/Customer/Orders/5 - Xem đơn hàng của khách
        public ActionResult Orders(int id)
        {
            var customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            ViewBag.CustomerName = customer.CustomerName;
            var orders = db.Orders
                .Where(o => o.CustomerID == id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
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

