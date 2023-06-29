using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Dtos
{
    public class UserFirmDto
    {

        public string Account { get; set; }  

        public string LastName { get; set; }  //姓

        public string FirstName { get; set; }  //名

        public string Phone { get; set; }

        //廠商
        public FirmDto Firms { get; set; }

    }
}
