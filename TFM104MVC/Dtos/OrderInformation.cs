using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Dtos
{
    public class OrderInformation
    {
        public List<FromCart> CheckOutList { get; set; }
        //public string UserInformation { get; set; }
        public UserInfo UserInformation { get; set; }
    }
}
