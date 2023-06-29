using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Dtos
{
    public class UserForUpdate
    {
        public string LastName { get; set; }  //姓
        public string FirstName { get; set; }  //名
        public string Phone { get; set; }
        public Member Members { get; set; }
    }
}
