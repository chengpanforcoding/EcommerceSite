using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Dtos
{
    public class LoginDto
    {
        [Required]
        public string Account { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
