using AspNetCore.Identity.MongoDbCore.Infrastructure;
using MongoDB.Driver;
using PizzaApp.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PizzaApp.Service
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _orders = database.GetCollection<Order>("Order");
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _orders.Find(order => true).ToListAsync();
        }

        public async Task<Order> GetByIdAsync(string id)
        {
            return await _orders.Find(o => o.Id == id).FirstOrDefaultAsync<Order>();
        }

        public async Task<bool> CreateAsync(Order order)
        {
            if (order == null)
                return false;
            try
            {
                await _orders.InsertOneAsync(order);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateAsync(string id, Order order)
        {
            if (order == null)
                return false;
            try
            {
                await _orders.ReplaceOneAsync(o => o.Id == id, order);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveAsync(Order order)
        {
            if (order == null)
                return false;
            try
            {
                await _orders.DeleteOneAsync(o => o.Id == order.Id);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
