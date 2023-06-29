using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Models.Session
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public double? DiscountPersent { get; set; }
        public int Quantity { get; set; }
    }
}
