using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Models.Entity
{
    public class Member
    {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }   //拿使用者編號(外來鍵)當主鍵
        public virtual User User { get; set; }

        //生日
        public DateTime? Birthday { get; set; }

        //性別
        public bool? Gender { get; set; }

        //圖像
        public string PicPath { get; set; } //圖片路徑
    }
}
