using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Porto.Models;

namespace Porto.Controllers
{
    public class CartController : Controller
    {
        private PortoEntities db = new PortoEntities();

        public ActionResult Index()
        {
            var cart = CartSession.GetCart();
            return View(cart);
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = db.Products.Find(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            var item = new CartItem
            {
                ProductID = product.ProductID,
                Name = product.ProductName,
                Price = product.ProductPrice,
                Quantity = quantity,
                Image = product.ProductImage
            };

            CartSession.AddToCart(item);

            return Json(new
            {
                success = true,
                count = CartSession.GetCount(),
                total = CartSession.GetTotal().ToString("C")
            });
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            CartSession.UpdateQuantity(productId, quantity);
            var total = CartSession.GetTotal();
            return Json(new { success = true, total = total.ToString("C") });
        }

        [HttpPost]
        public ActionResult RemoveItem(int productId)
        {
            CartSession.RemoveFromCart(productId);
            var total = CartSession.GetTotal();
            return Json(new { success = true, total = total.ToString("C"), count = CartSession.GetCount() });
        }

        public ActionResult Clear()
        {
            CartSession.ClearCart();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetCount()
        {
            var count = CartSession.GetCount();
            return Json(count, JsonRequestBehavior.AllowGet);
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