using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OrderService.Context;
using OrderService.Dtos.Response;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IHttpClientFactory httpClientFactory;

        public OrderController(DataContext _context, IHttpClientFactory _httpClientFactory)
        {
            this.httpClientFactory = _httpClientFactory;
            this.context = _context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order request)
        {
            var UserId = int.Parse(User.FindFirst("id")!.Value);
            var accessToken = Request.Headers["Authorization"].ToString();


            var client = httpClientFactory.CreateClient("ProductService");

            var orderItems = new List<OrderItem>();

            foreach(var item in request.Items)
            {
                var productRequest = new HttpRequestMessage(HttpMethod.Get, $"/product/{item.ProductId}");

                productRequest.Headers.Add("Authorization", accessToken);

                var response = await client.SendAsync(productRequest);

                if (!response.IsSuccessStatusCode)
                    return BadRequest($"Product {item.ProductId} not found or unauthorized.");

                var productResponse = await response.Content.ReadFromJsonAsync<ProductResponse>();

                if(productResponse == null)
                    return BadRequest($"Product {item.ProductName} not found.");

                orderItems.Add(new OrderItem 
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity
                });
            }

            var order = new Order
            {
                CreateBy = UserId,
                Items = orderItems,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };


            await context.Orders.InsertOneAsync(order);
            return Ok(new { response = "บันทึกข้อมูลสำเร็จ" });
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrderByUser()
        {
            var UserId = int.Parse(User.FindFirst("id")!.Value);
            var order = await context.Orders.Find(x => x.CreateBy == UserId).ToListAsync();
            return Ok(new { response = order});
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(string id)
        {
            var order = await context.Orders.Find(o => o.Id == id).FirstOrDefaultAsync();
            var Model = new object();
            var ItemListModels = new List<object>();

            foreach(var _order in order.Items)
            {   
                ItemListModels.Add(new 
                {
                    ProductName = _order.ProductName,
                    Price = _order.Price,
                    Quantity = _order.Quantity,
                    Total = _order.Price * _order.Quantity
                });
            }
            
            Model = new 
            {
                customer = order.CreateBy,
                TotalPrice = order.Total,
                Items = ItemListModels
            };
            return order == null ? NotFound(new {response = "ไม่พบข้อมูล"}) : Ok(new { response = Model});
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await context.Orders.DeleteOneAsync(o => o.Id == id);
            return result.DeletedCount == 0 ? NotFound() : NoContent();
        }
    }
}