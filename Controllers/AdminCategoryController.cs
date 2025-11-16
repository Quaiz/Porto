using System;
using System.Linq;
using System.Web.Mvc;
using Porto.Models;

namespace Porto.Controllers
{
    public class AdminCategoryController : AdminBaseController
    {
        private PortoEntities db = new PortoEntities();

        public ActionResult Index()
        {
            var categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
            return View(categories);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var category = db.Categories.Find(id);
            if (category != null)
            {
                // Check if category has products
                var hasProducts = db.Products.Any(p => p.CategoryID == id);
                if (hasProducts)
                {
                    TempData["Error"] = "Cannot delete category because it has products. Please delete or reassign the products first.";
                    return RedirectToAction("Index");
                }

                db.Categories.Remove(category);
                db.SaveChanges();
                TempData["Success"] = "Category deleted successfully.";
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