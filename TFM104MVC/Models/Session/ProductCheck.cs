using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Models.Session
{
    public class ProductCheck
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PayMethod { get; set; }
    }
}
