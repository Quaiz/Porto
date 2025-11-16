using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Porto.Models;

namespace Porto.Controllers
{
    public class AdminProductController : AdminBaseController
    {
        private PortoEntities db = new PortoEntities();

        public ActionResult Index()
        {
            var products = db.Products.Include("Category").OrderBy(p => p.ProductName).ToList();
            return View(products);
        }

        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var uniqueName = Guid.NewGuid().ToString() + "_" + fileName;
                    var imagesPath = Server.MapPath("~/Content/images/");
                    
                    // Ensure directory exists
                    if (!Directory.Exists(imagesPath))
                    {
                        Directory.CreateDirectory(imagesPath);
                    }
                    
                    var path = Path.Combine(imagesPath, uniqueName);
                    imageFile.SaveAs(path);
                    product.ProductImage = uniqueName;
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryID);
            return View(product);
        }

        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var existing = db.Products.AsNoTracking().FirstOrDefault(p => p.ProductID == product.ProductID);

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var imagesPath = Server.MapPath("~/Content/images/");
                    
                    if (!string.IsNullOrEmpty(existing.ProductImage))
                    {
                        var oldPath = Path.Combine(imagesPath, existing.ProductImage);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    // Ensure directory exists
                    if (!Directory.Exists(imagesPath))
                    {
                        Directory.CreateDirectory(imagesPath);
                    }

                    var fileName = Path.GetFileName(imageFile.FileName);
                    var uniqueName = Guid.NewGuid().ToString() + "_" + fileName;
                    var path = Path.Combine(imagesPath, uniqueName);
                    imageFile.SaveAs(path);
                    product.ProductImage = uniqueName;
                }
                else
                {
                    product.ProductImage = existing.ProductImage;
                }

                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        public ActionResult Delete(int id)
        {
            var product = db.Products.Include("Category").FirstOrDefault(p => p.ProductID == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                // Check if product is in any orders
                var hasOrders = db.OrderDetails.Any(od => od.ProductID == id);
                if (hasOrders)
                {
                    TempData["Error"] = "Cannot delete product because it exists in order history.";
                    return RedirectToAction("Index");
                }

                if (!string.IsNullOrEmpty(product.ProductImage))
                {
                    var path = Path.Combine(Server.MapPath("~/Content/images/"), product.ProductImage);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["Success"] = "Product deleted successfully.";
            }
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