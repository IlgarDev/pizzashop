using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PizzaApp.Helpers;
using PizzaApp.Models;
using PizzaApp.Models.Entities;
using PizzaApp.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaApp.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly TicketService ticketService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly FoodService foodService;
        private readonly OrderService orderService;

        public CheckoutController(TicketService ticketService, UserManager<ApplicationUser> userManager, FoodService foodService, OrderService orderService)
        {
            this.ticketService = ticketService;
            this.userManager = userManager;
            this.foodService = foodService;
            this.orderService = orderService;
        }
        public IActionResult Index()
        {
            var list = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

            return View(list);
        }

        public async Task<IActionResult> Buy()
        {
            var list = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            var ticket = new Ticket()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Buyer = await userManager.GetUserAsync(User),
                Items = list
            };
            if (await ticketService.CreateAsync(ticket))
            {
                decimal sum = 0;
                var order = new Order();
                order.isReady = false;
                order.Foods = new List<OrderFood>();
                foreach (var item in list)
                {
                    var product = item.Food;
                    if (product.Price > ticket.Buyer.Balance) return RedirectToAction("Index", "Home");
                    product.Quantity -= item.Quantity;
                    if (!order.Foods.Exists(o => o.FoodId == item.Food.Id))
                    {
                        order.Foods.Add(new OrderFood
                        {
                            FoodId = item.Food.Id,
                            FoodName = item.Food.Name,
                            Quantity = item.Quantity
                        });
                    }
                    order.BuyerId = ticket.Buyer.Id.ToString();
                    order.BuyerName = $"{ticket.Buyer.FirstName}_{ticket.Buyer.LastName}({ticket.Buyer.UserName})";
                    sum += item.Quantity * item.Food.Price;
                    await foodService.UpdateAsync(item.Food.Id, product);
                }
                ticket.Buyer.Balance -= sum;
                await userManager.UpdateAsync(ticket.Buyer);
                await orderService.CreateAsync(order);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "dcart", null);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
    }
}
