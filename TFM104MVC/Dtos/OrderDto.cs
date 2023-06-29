using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models.Entity;
using TFM104MVC.Models.Enum;
using TFM104MVC.Models.Session;

namespace TFM104MVC.Dtos
{
    public class OrderDto
    {
        public string Name { get; set; } //訂購人姓名

        public string Phone { get; set; } //電話

        public string Email { get; set; } //信箱

        public DateTime Date { get; set; } //購買日期

        public double? Discount { set; get; } //平台折扣

        public OrderStatus OrderStatus { set; get; } //訂單狀態

        public virtual ICollection<Orderdetail> Orderdetails { get; set; }

        //一個使用者會有多個訂單
        public int UserId { get; set; } //使用者編號
    }
}
