using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Dtos
{
    public class BankOrder
    {
        public string ordernumber { get; set; }
        public int amount { get; set; }
        public string PayMethod { get; set; }
    }
}
