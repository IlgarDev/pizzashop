using AspNetCore.Identity.MongoDbCore.Infrastructure;
using MongoDB.Driver;
using PizzaApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaApp.Service
{
    public class TicketService
    {
        private readonly IMongoCollection<Ticket> _ticket;

        public TicketService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _ticket = database.GetCollection<Ticket>("Tickets");
        }

        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _ticket.Find(ticket => true).ToListAsync();
        }

        public async Task<Ticket> GetByIdAsync(string id)
        {
            return await _ticket.Find(ticket => ticket.Id == id).FirstOrDefaultAsync<Ticket>();
        }

        public async Task<bool> CreateAsync(Ticket ticket)
        {
            if (ticket == null)
                return false;
            try
            {
                await _ticket.InsertOneAsync(ticket);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateAsync(string id, Ticket ticket)
        {
            if (ticket == null)
                return false;
            try
            {
                await _ticket.ReplaceOneAsync(x => x.Id == id, ticket);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveAsync(Ticket ticket)
        {
            if (ticket == null)
                return false;
            try
            {
                await _ticket.DeleteOneAsync(x => x.Id == ticket.Id);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
