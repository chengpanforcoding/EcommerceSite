using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFM104MVC.Models.Entity
{
    public class CustomerRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } //評價編號

        [Required]
        [MaxLength(50)]
        public string Title { get; set; } //評價標題

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } //評價內容

        [Range(1, 5)]
        public int Score { get; set; } //評價分數

        [Required]
        public DateTime CreateDate { get; set; } //建立日期
        [Required]
        public DateTime UpdateTime { get; set; } //更新時間

        //一個商品擁有多個評價 一對多關係
        [ForeignKey("Product")] //外來鍵
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        //public Guid MemberId { get; set; }
        //public Member Member { get; set; }

        //與訂單明細是一對一關係 但評價可以不用包含訂單明細資訊
        //[ForeignKey("Orderdetail")] //外來鍵
        //public Guid OrderdetailId { get; set; }
        public virtual Orderdetail Orderdetail { get; set; }
    }
}