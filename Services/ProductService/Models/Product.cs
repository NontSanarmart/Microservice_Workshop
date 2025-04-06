using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? CreateBy { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public int? UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;
    }
}