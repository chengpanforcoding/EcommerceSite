using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Models.Session
{
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public ProductDto Product { get; set; }
        public decimal OriginalPrice { get; set; }
        public double? DiscountPersent { get; set; }
        public int Quantity { get; set; }
        //public ProductPicture ProductPictures { get; set; }
    }
}
