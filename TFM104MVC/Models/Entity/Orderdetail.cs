using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFM104MVC.Models.Entity
{
    public class Orderdetail
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }  //商品單價金額

        [Required]
        public int Quantity { get; set; }  //商品數量

        [Range(0.0, 1.0)]
        public double? DiscountPersent { get; set; } //商品折扣(廠商)

        //[Key]
        //[Column(Order = 1)]
        //外來鍵商品id
        //[ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        //[Key]
        //[Column(Order = 2)]
        //外來鍵訂單id
        //[ForeignKey("Order")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        //    //與商品評價 一對一關係
        //    [ForeignKey("CustomerRating")]
        //    public int RateId { get; set; } //評價編號
        //    public virtual CustomerRating CustomerRating { get; set; }
    }
}