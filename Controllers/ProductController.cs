using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Porto.Models;

namespace Porto.Controllers
{
    public class ProductController : Controller
    {
        private PortoEntities db = new PortoEntities();

        public ActionResult Index(string search = "", string sort = "name", int page = 1, int category = 0)
        {
            int pageSize = 12;
            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.ProductName.Contains(search));
            }

            if (category > 0)
            {
                products = products.Where(p => p.CategoryID == category);
            }

            switch (sort)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }

            var totalItems = products.Count();
            var items = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.Category = category;
            ViewBag.Categories = db.Categories.ToList();

            return View(items);
        }

        public ActionResult Details(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
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