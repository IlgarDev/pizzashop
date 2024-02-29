using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PizzaApp.Models.Entities  
{
    public class Food
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Currency)]
        [Range(0, 9999999999.99)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageContentType { get; set; }
    }
}
