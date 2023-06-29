using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Dtos
{
    public class checkOrderInfoDto
    {
        public string ordernumber { get; set; } //訂單編號
        public int amount { get; set; } //訂單總金額
        public string PayMethod { get; set; } //付款方式
    }
}
