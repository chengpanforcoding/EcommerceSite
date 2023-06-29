using System;

namespace TFM104MVC.Dtos
{
    public class OrderdetailDto
    {
        public decimal UnitPrice { get; set; }  //商品單價金額
        public int Quantity { get; set; }  //商品數量
        public double? DiscountPersent { get; set; } //商品折扣(廠商)
        public Guid ProductId { get; set; }
        public ProductDto Product { get; set; }
        public int OrderId { get; set; }
        public int RateId { get; set; } //評價編號


    }
}
