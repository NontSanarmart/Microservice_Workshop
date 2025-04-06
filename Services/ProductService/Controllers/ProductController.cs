using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Context;
using ProductService.Dtos.Request;
using ProductService.Models;

namespace ProductService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly DataContext context;

        public ProductController(DataContext _context)
        {
            context = _context;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var product = await context.Products.AsNoTracking().ToListAsync();
            return Ok(new { response = product });
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            return await context.Products.FindAsync(id) is Product product ? Ok(new { response = product }) : NotFound(new { message = "ไม่พบข้อมูล" });
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProduct request)
        {
            var UserId = int.Parse(User.FindFirst("id")!.Value);
            if (!ModelState.IsValid)
                return BadRequest(new { response = ModelState });

            var product = new Product
            {
                ProductName = request.ProductName,
                Price = request.Price,
                Description = request.Description,
                CreateBy = UserId,
                UpdateBy = UserId
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { message = "บันทึกข้อมูลสำเร็จ" });
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateProduct request)
        {
            var UserId = int.Parse(User.FindFirst("id")!.Value);

            if (id != request.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(new { response = ModelState });

            var product = await context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "ไม่พบสินค้า" });

            product.ProductName = request.ProductName;
            product.Price = request.Price;
            product.Description = request.Description;
            product.UpdateBy = UserId;
            product.UpdateDate = DateTime.UtcNow;

            context.Products.Update(product);
            await context.SaveChangesAsync();
            return Ok(new { message = "บันทึกข้อมูลสำเร็จ" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "ไม่พบสินค้า" });

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return Ok(new { message = "ลบข้อมูลสำเร็จ" });
        }
    }
}