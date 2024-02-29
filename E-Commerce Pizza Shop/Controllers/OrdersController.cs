using Microsoft.AspNetCore.Mvc;
using PizzaApp.Service;
using System.Threading.Tasks;

namespace PizzaApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService foodService)
        {
            _orderService = foodService;
        }

        public async Task<ActionResult> Index()
        {
            var order = await _orderService.GetAllAsync();
            return View(order);
        }

        public async Task<ActionResult> Update(string orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            order.isReady = true;
            await _orderService.UpdateAsync(orderId, order);
            return RedirectToAction("Index");
        }
    }
}
