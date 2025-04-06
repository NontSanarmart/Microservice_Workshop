using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Dtos.Request
{
    public class UpdateProduct
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "กรุณาระบุชื่อสินค้า")]
        [MaxLength(150, ErrorMessage = "ชื่อสินค้าต้องไม่เกิด 150 ตัวอักษร")]
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        [MaxLength(150, ErrorMessage = "คำอธิบายต้องไม่เกิน 500 ตัวอักษร")]
        public string Description { get; set; } = string.Empty;
    }
}