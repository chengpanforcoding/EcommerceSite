using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Models.Entity
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[Required(ErrorMessage = "帳號不可為空")]
        //[EmailAddress(ErrorMessage = "須符合Email格式")]

        [Required]
        [EmailAddress]
        public string Account { get; set; }         //1.使用者帳號 信箱

        [Required]
        public string Password { get; set; }        //2.使用者密碼 //SHA256

        //[Required]
        public string Salt { get; set; }    //加鹽

        public string LastName { get; set; }  //姓

        public string FirstName { get; set; }  //名

        //5.電話
        [Phone]
        public string Phone { get; set; }

        //信箱驗證
        public bool Verification { get; set; }

        [ForeignKey("Role")]
        public string RoleName { get; set; }
        public virtual Role Role { get; set; }

        //一對一關係 不包含信息 
        //廠商
        public virtual Firm Firms { get; set; }

        //管理員
        public virtual Admin Admins { get; set; }

        //會員
        public virtual Member Members { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
