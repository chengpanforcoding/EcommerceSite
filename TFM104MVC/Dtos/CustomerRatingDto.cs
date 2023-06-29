using System;

namespace TFM104MVC.Dtos
{
    public class CustomerRatingDto
    {
        public int Id { get; set; } //評價編號
        public string Title { get; set; } //評價標題
        public string Content { get; set; } //評價內容
        public int Score { get; set; } //評價分數
        public DateTime CreateDate { get; set; } //建立日期
        public DateTime UpdateTime { get; set; } //更新時間
        public Guid ProductId { get; set; }

    }
}
