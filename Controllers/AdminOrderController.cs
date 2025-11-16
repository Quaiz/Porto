using System;
using System.Linq;
using System.Web.Mvc;
using Porto.Models;

namespace Porto.Controllers
{
    public class AdminOrderController : AdminBaseController
    {
        private PortoEntities db = new PortoEntities();

        public ActionResult Index()
        {
            var orders = db.Orders.Include("Customer").OrderByDescending(o => o.OrderDate).ToList();
            return View(orders);
        }

        public ActionResult Details(int id)
        {
            var order = db.Orders
                .Include("Customer")
                .Include("OrderDetails")
                .Include("OrderDetails.Product")
                .FirstOrDefault(o => o.OrderID == id);
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