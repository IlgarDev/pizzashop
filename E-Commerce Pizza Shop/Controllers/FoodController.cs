using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using PizzaApp.Models.Entities;
using PizzaApp.Service;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PizzaApp.Controllers
{
    public class FoodController : Controller
    {
        private readonly FoodService _foodService;

        public FoodController(FoodService foodService)
        {
            _foodService = foodService;
        }

        public async Task<ActionResult> Index(int page = 1, int pageSize = 3)
        {
            var foods = await _foodService.GetPaginatedAsync(page, pageSize);
            var totalCount = await _foodService.GetAllAsync();
            var totalItemCount = totalCount.Count;
            var totalPages = (int)Math.Ceiling((double)totalItemCount / pageSize);
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            return View(foods);
        }

        public async Task<ActionResult> Search(string foodname)
        {
            var food = await _foodService.SearchFood(foodname);
            if (foodname == string.Empty || foodname == null)
            {
                food = await _foodService.GetAllAsync();
                return View("Index", food);
            }
            return View("Index", food);
        }

        public async Task<ActionResult> Details(string id)
        {
            var food = await _foodService.GetByIdAsync(id);
            if (food == null)
                return NotFound();
            return View(food);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Create(Food food, IFormFile imageData)
        {
            if (ModelState.IsValid && imageData != null && imageData.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageData.CopyToAsync(memoryStream);
                    food.ImageData = memoryStream.ToArray();
                    food.ImageContentType = imageData.ContentType;
                }

                food.Id = ObjectId.GenerateNewId().ToString();
                await _foodService.CreateAsync(food);
                return RedirectToAction("Index");
            }
            return View(food);
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Edit(string id)
        {
            var food = await _foodService.GetByIdAsync(id);
            if (food == null)
                return NotFound();

            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Edit(string id, Food food)
        {
            if (ModelState.IsValid)
            {
                food.Id = id;
                await _foodService.UpdateAsync(id, food);
                return RedirectToAction("Index");
            }
            return View(food);
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(string id)
        {
            var food = await _foodService.GetByIdAsync(id);
            if (food == null)
                return NotFound();

            return View(food);
        }


        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Food food = await _foodService.GetByIdAsync(id);
            await _foodService.RemoveAsync(food);
            return RedirectToAction("Index");
        }
    }
}
