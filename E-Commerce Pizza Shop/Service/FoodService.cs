using AspNetCore.Identity.MongoDbCore.Infrastructure;
using MongoDB.Driver;
using PizzaApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PizzaApp.Service
{
    public class FoodService
    {
        private readonly IMongoCollection<Food> _food;

        public FoodService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _food = database.GetCollection<Food>("Food");
        }

        public async Task<List<Food>> GetAllAsync()
        {
            return await _food.Find(food => true).ToListAsync();
        }

        public async Task<List<Food>> GetPaginatedAsync(int page, int pageSize)
        {
            var foods = await _food.Find(_ => true)
                                    .Skip((page - 1) * pageSize)
                                    .Limit(pageSize)
                                    .ToListAsync();
            return foods;
        }

        public async Task<Food> GetByIdAsync(string id)
        {
            return await _food.Find(f => f.Id == id).FirstOrDefaultAsync<Food>();
        }

        public async Task<bool> CreateAsync(Food food)
        {
            if (food == null)
                return false;
            try
            {
                await _food.InsertOneAsync(food);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateAsync(string id, Food food)
        {
            if (food == null)
                return false;
            try
            {
                await _food.ReplaceOneAsync(x => x.Id == id, food);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveAsync(Food food)
        {
            if (food == null)
                return false;
            try
            {
                await _food.DeleteOneAsync(x => x.Id == food.Id);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<List<Food>> SearchFood(string foodname)
        {
            var food = await _food.Find(f => f.Name == foodname).ToListAsync();
            return food;
        }
    }
}
