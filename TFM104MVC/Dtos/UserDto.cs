using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Dtos
{
    public class UserMemberDto
    {
        public int Id { get; set; }

        public string Account { get; set; }         //1.使用者帳號 信箱

        public string Password { get; set; }        //2.使用者密碼 //SHA256

        public string Salt { get; set; }    //加鹽

        public string LastName { get; set; }  //姓

        public string FirstName { get; set; }  //名

        public string Phone { get; set; }

        public bool Verification { get; set; }

        public string RoleName { get; set; }
        //會員
        public  MemberDto Members { get; set; }

        public FirmDto Firms { get; set; }

    }
}
