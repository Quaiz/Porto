using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Porto.Models;

namespace Porto.Controllers
{
    public class HomeController : Controller
    {
        private PortoEntities db = new PortoEntities();

        public ActionResult Index()
        {
            var products = db.Products.OrderByDescending(p => p.ProductID).Take(8).ToList();
            return View(products);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
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