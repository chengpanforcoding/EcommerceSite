using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Models.Entity;
using TFM104MVC.Services;

namespace TFM104MVC.Controllers
{
    public class ProductViewController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductViewController(IProductRepository productRepository,IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        //全部商品列表
        public IActionResult ProductList()
        {
            return View();
        }

        //單一商品
        public IActionResult Product([FromRoute] Guid id)  //商品頁後面接商品id
        {
            //把商品id存進session裡面，準備之後傳到其他地方(訂購頁,購物車 etc...)用
            //HttpContext.Session.SetString("pid", id.ToString());

            //HttpContext.Session.GetString("camping_area_id");
            //Convert.ToInt32()
            var a = _productRepository.GetProductTitle(id);
            ViewBag.message = a;
            return View();
        }
    }
}
