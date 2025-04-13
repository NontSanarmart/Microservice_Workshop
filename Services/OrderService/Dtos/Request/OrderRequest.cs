using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Dtos.Request
{
    public class OrderRequest
    {
        public List<OrderRequestItem> Items { get; set; } = new();
    }


    public class OrderRequestItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}