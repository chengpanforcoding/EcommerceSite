using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TFM104MVC.Models.Entity
{
    public class ProductPicture
    {
        [KeyAttribute]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Url { get; set; }
        
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        //連接屬性
        public virtual Product Product { get; set; }
    }
}
