using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Dtos
{
    public class DirectPurchase
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
