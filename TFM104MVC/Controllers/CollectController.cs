using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Controllers
{
    public class CollectController : Controller
    {
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult WishList()
        {
            return View();
        }
    }
}
