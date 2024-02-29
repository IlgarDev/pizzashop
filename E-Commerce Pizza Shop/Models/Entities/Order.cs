using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace PizzaApp.Models.Entities
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<OrderFood> Foods { get; set; }
        public string SummaryPrice { get; set; }
        public string PaymentStatus { get; set; }
        public string BuyerName { get; set; }
        public string BuyerId { get; set; }
        public bool isReady { get; set; }
    }
}
