using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Models.Entity
{
    public class Role
    {
        public int Id { get; set; }
        [Key]
        public string Name { get; set; }   //權限名稱

        //一個角色擁有多個使用者
        public virtual ICollection<User> Users { get; set; }
    }
}
