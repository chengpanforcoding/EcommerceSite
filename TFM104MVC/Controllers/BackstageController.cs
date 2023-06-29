using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Controllers
{
    //後台管理
    public class BackstageController : Controller
    {
        //後台登入頁
        public IActionResult Login()
        {
            return PartialView();
        }

        //後台註冊頁
        public IActionResult Register()
        {
            return PartialView();
        }

        [Authorize(Roles ="Firm,Admin")]
        //後台總覽儀錶板頁
        public IActionResult Home()
        {
            ViewBag.message = "後台總覽儀錶板";
            return View();
        }
        [Authorize(Roles = "Firm,Admin")]
        //商品列表頁
        public IActionResult ProductList()
        {
            return View();
        }
        [Authorize(Roles = "Firm,Admin")]
        //新增商品
        public IActionResult NewProduct()
        {
            return View();
        }
        [Authorize(Roles = "Firm,Admin")]
        //修改商品
        public IActionResult EditProduct()
        {
            return View();
        }
        [Authorize(Roles = "Firm,Admin")]
        //管理中心
        public IActionResult Admin()
        {
            return View();
        }
        [Authorize(Roles = "Firm,Admin")]
        //訂單列表
        public IActionResult OrderList()
        {
            return View();
        }
        [Authorize(Roles = "Firm,Admin")]
        //單筆訂單
        [Route("Backstage/order/{orderId}")]
        public IActionResult Order([FromRoute] int orderId)
        {
            return View();
        }

    }
}
