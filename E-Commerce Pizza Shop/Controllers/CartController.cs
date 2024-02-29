using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PizzaApp.Helpers;
using PizzaApp.Models.Entities;
using PizzaApp.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaApp.Controllers
{
    public class CartController : Controller
    {
        private readonly FoodService foodService;

        public CartController(FoodService foodService)
        {
            this.foodService = foodService;
        }

        [Route("index")]
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                foreach(var item in cart)
                {
                    var food = foodService.GetByIdAsync(item.Food.Id);
                    item.Food.ImageData = food.Result.ImageData;
                }
                ViewBag.cart = cart;
                ViewBag.total = cart.Sum(item => item.Food.Price * item.Quantity);
            }
            return View();
        }

        [Route("buy/{id}")]
        public async Task<IActionResult> Buy(string id)
        {
            var product = await foodService.GetByIdAsync(id);
            if (product.Quantity < 1)
                return RedirectToAction("Index", "Product");

            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { Food = product, Quantity = 1 });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Item { Food = product, Quantity = 1 });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }

        [Route("remove/{id}")]
        public IActionResult Remove(string id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            if (cart.Count == 0)
                cart = null;
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult subOne(string id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            if (index != -1 && cart[index].Quantity > 1)
                cart[index].Quantity--;
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult addOne(string id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            if (index != -1 && cart[index].Quantity < cart[index].Food.Quantity)
                cart[index].Quantity++;
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

            return RedirectToAction("Index");
        }

        private int isExist(string id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Food.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
