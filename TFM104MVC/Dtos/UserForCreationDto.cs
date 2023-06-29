using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Dtos
{
    public class UserForCreationDto
    {
        [Required(ErrorMessage ="帳號不可為空")]
        [EmailAddress(ErrorMessage ="帳號須符合信箱格式")]
        public string Account { get; set; }         //1.使用者帳號 信箱

        [Required(ErrorMessage ="密碼不可為空")]
        public string Password { get; set; }        //2.使用者密碼 //SHA256

        public string LastName { get; set; }  //名

        public string FirstName { get; set; }  //姓

        [Phone(ErrorMessage ="電話格式不符")]

        public string Phone { get; set; }

        //信箱驗證
        public bool Verification { get; set; }

        public string RoleName { get; set; }

        //一對一關係 不包含信息 
        //廠商
        public virtual Firm Firms { get; set; }

        //管理員
        public virtual Admin Admins { get; set; }

        //會員
        public virtual Member Members { get; set; }
    }
}
