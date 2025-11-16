using System;
using System.Linq;
using System.Web.Mvc;
using Porto.Models;

namespace Porto.Controllers
{
    public class AdminInventoryController : AdminBaseController
    {
        private PortoEntities db = new PortoEntities();

        // GET: Admin/Inventory - Danh sách hàng tồn
        public ActionResult Index(string search = "", string sort = "name")
        {
            var products = db.Products.Include("Category").AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.ProductName.Contains(search) || p.Category.CategoryName.Contains(search));
            }

            switch (sort)
            {
                case "quantity_asc":
                    products = products.OrderBy(p => p.ProductQuantity);
                    break;
                case "quantity_desc":
                    products = products.OrderByDescending(p => p.ProductQuantity);
                    break;
                case "price":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }

            ViewBag.Search = search;
            ViewBag.Sort = sort;
            return View(products.ToList());
        }

        // GET: Admin/Inventory/LowStock - Hàng sắp hết
        public ActionResult LowStock(int threshold = 10)
        {
            var products = db.Products
                .Include("Category")
                .Where(p => p.ProductQuantity <= threshold)
                .OrderBy(p => p.ProductQuantity)
                .ToList();

            ViewBag.Threshold = threshold;
            return View(products);
        }

        // GET: Admin/Inventory/UpdateStock/5
        public ActionResult UpdateStock(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Inventory/UpdateStock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStock(int id, int quantity, string action)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            if (action == "add")
            {
                product.ProductQuantity += quantity;
            }
            else if (action == "set")
            {
                product.ProductQuantity = quantity;
            }

            db.SaveChanges();
            TempData["Success"] = $"Đã cập nhật số lượng tồn kho cho {product.ProductName}";
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

