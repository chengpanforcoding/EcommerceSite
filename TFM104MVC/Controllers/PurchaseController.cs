using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using TFM104MVC.Dtos;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;
using TFM104MVC.Services;

namespace TFM104MVC.Controllers
{
    public class PurchaseController : Controller
    {
        private IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public PurchaseController(IProductRepository productRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _environment = webHostEnvironment;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult Booking([FromForm]List<string> productId)
        {
            List<string> pidList= new List<string>();
            ViewBag.pidList = productId;

            //Console.WriteLine(ViewBag.pidList);

            return View();

        }

        [Authorize(AuthenticationSchemes ="Cookies")]
        public IActionResult BookingNow([FromQuery] string productId,int quantity)
        {
            Console.WriteLine(productId);
            Console.WriteLine(quantity);

            return View();
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult Cart()
        {
            return View();
        }

    }
}
