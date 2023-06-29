using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Dtos
{
    public class OrderForShowDto
    {
        public int Id { get; set; } //訂單編號

        public string Name { get; set; } //訂購人姓名

        public string Phone { get; set; } //電話
        
        public string Email { get; set; } //信箱

        public string Date { get; set; } //購買日期

        public double? Discount { set; get; } //平台折扣

        public string OrderStatus { set; get; } //訂單狀態

        public ICollection<OrderdetailDto> Orderdetails { get; set; }

        public int UserId { get; set; } //使用者編號
    }
}
