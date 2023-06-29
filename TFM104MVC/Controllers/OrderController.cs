using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TFM104MVC.Controllers
{
    public class OrderController : Controller
    {
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult Manage()
        {
            return View();
        }

        [Route("order/detail/{orderId}/{productId}")]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult Detail([FromRoute]int orderId,Guid productId)
        {
            return View();
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}
