using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Porto.Models
{
    public class CartSession
    {
        private const string CartSessionKey = "Cart";

        public static List<CartItem> GetCart()
        {
            var cart = HttpContext.Current.Session[CartSessionKey] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                HttpContext.Current.Session[CartSessionKey] = cart;
            }
            return cart;
        }

        public static void AddToCart(CartItem item)
        {
            var cart = GetCart();
            var existing = cart.Find(c => c.ProductID == item.ProductID);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }
        }

        public static void UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.Find(c => c.ProductID == productId);
            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }
        }

        public static void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.Find(c => c.ProductID == productId);
            if (item != null)
                cart.Remove(item);
        }

        public static void ClearCart()
        {
            var cart = GetCart();
            cart.Clear();
        }

        public static decimal GetTotal()
        {
            var cart = GetCart();
            decimal total = 0;
            foreach (var item in cart)
            {
                total += item.Total;
            }
            return total;
        }

        public static int GetCount()
        {
            var cart = GetCart();
            int count = 0;
            foreach (var item in cart)
            {
                count += item.Quantity;
            }
            return count;
        }
    }
}