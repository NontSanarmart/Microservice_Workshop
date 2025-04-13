using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Price * i.Quantity);
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public int? UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;
    }


    public class OrderItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}