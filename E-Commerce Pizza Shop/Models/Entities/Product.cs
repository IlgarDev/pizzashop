using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaApp.Models.Entities
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Currency)]
        [Range(0, 9999999999.99)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
