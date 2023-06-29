using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models.Enum;

namespace TFM104MVC.Models.Entity
{
    public class Product
    {
        [KeyAttribute]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")] //代表可以輸入到 18位數 至 小數點後兩位
        public decimal OriginalPrice { get; set; }

        [Range(0.0, 1.0)] //範圍只能介於0~1
        public double? DiscountPersent { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? GoTouristTime { get; set; }

        [MaxLength(1500)]
        public string Notes { get; set; }

        public ProductStatus? ProductStatus { get; set; } //商品狀態
        public double? CustomerRating { get; set; }
        public TravelDays? TravelDays { get; set; }
        public TripType? TripType { get; set; }
        public Region? Region { get; set; }
        public ICollection<ProductPicture> ProductPictures { get; set; }//導覽屬性(從一個商品會有多個商品圖片可以參觀~)
                = new List<ProductPicture>();

        //成為別人的外來鍵 在自己家裡面一定是主索引鍵

        //public virtual ICollection<CustomerRating> CustomerRatings { get; set; } //商品評價
        //      = new List<CustomerRating>();
        ////一個商品擁有多個評價

        public virtual ICollection<Orderdetail> Orderdetails { get; set; }

        public static implicit operator Product(Task<Product> v)
        {
            throw new NotImplementedException();
        }
        //訂單與商品多對多 延伸出的關係   //一個商品擁有多個訂單明細

    }
}
