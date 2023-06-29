using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Models.Entity
{
    public class Firm
    {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }


        public string TaxId { get; set; }   //公司統編
        public string Name { get; set; } //公司名稱
    }
}
