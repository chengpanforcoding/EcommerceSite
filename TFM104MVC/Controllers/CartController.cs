using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Helpers;
using TFM104MVC.Models.Entity;
using TFM104MVC.Services;

namespace TFM104MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CartController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        [HttpPost("addcart")]
        //[Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult AddCart([FromBody] AddCartItemDto addCartItemDto)
        {
            //判斷Session內有沒有購物車
            if (SessionHelper.GetObjectFromJson<List<AddCartItemDto>>(HttpContext.Session, "cart") == null)
            {
                //如果沒有已存在購物車 則建立一個購物車給使用者
                List<AddCartItemDto> cart = new List<AddCartItemDto>();
                cart.Add(addCartItemDto);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                return Ok();
            }
            else
            {
                //如果已經存在購物車 就直接把新的item加上去
                //先檢查有沒有相同商品 如果有相同商品則只修改數量
                List<AddCartItemDto> cart = SessionHelper.GetObjectFromJson<List<AddCartItemDto>>(HttpContext.Session, "cart");
                int index = cart.FindIndex(x => x.ProductId == addCartItemDto.ProductId);
                if (index != -1)
                {
                    cart[index].Quantity += addCartItemDto.Quantity;
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                }
                else
                {
                    cart.Add(addCartItemDto);
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                }
                return Ok();
            }
        }
        [HttpPost("removecart")]
        //[Authorize(AuthenticationSchemes ="Cookies")]
        public IActionResult RemoveCart([FromBody] RemoveCartItemDto removeCartItemDto)
        {
            //向Session取得內容
            List<AddCartItemDto> cart = SessionHelper.GetObjectFromJson<List<AddCartItemDto>>(HttpContext.Session, "cart");

            //查詢要刪除的商品在List裡面的哪一個位置
            int index = cart.FindIndex(x => x.ProductId == removeCartItemDto.ProductId);
            cart.RemoveAt(index);

            //檢查購物車內是否還有商品
            //如果沒有商品就清除Session
            if (cart.Count() < 1)
            {
                SessionHelper.Remove(HttpContext.Session, "cart");
            }
            else //如果還有商品就重新設定Session內容 把更新後的內容設定上去
            {
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }

            return NoContent();
        }

        //[HttpGet]
        //[Authorize(AuthenticationSchemes = "Cookies")]
        //public IActionResult GetCart()
        //{
        //    var cart = SessionHelper.GetObjectFromJson<List<AddCartItemDto>>(HttpContext.Session, "cart");
        //    if (cart == null || cart.Count() == 0)
        //    {
        //        return Ok(new List<AddCartItemDto>);
        //    }
        //    else
        //    {
        //        return Ok(cart);
        //    }
        //}

        [HttpGet("GetFullInfoCart")]
        //[Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<IActionResult> GetFullInfoCartAsync()
        {
            var cart = SessionHelper.GetObjectFromJson<List<AddCartItemDto>>(HttpContext.Session, "cart");
            if (cart == null || cart.Count() == 0)
            {
                return Ok(new List<AddCartItemDto>());
            }
            else
            {
                var allProd = await _productRepository.GetProductsByIds(cart.Select(x => x.ProductId).ToArray());
                //var data = allProd.Select(x => new
                //{
                //    x.Id,
                //    x.Title,
                //    x.GoTouristTime,
                //    x.Description,
                //    x.OriginalPrice,
                //    x.DiscountPersent,
                //    qty = cart.First(c=>c.ProductId == x.Id).Quantity
                //});

                var productdto = _mapper.Map<List<ProductForCartDto>>(allProd);

                foreach (var i in productdto)
                {
                    i.Title = i.Title.Substring(0, 20);
                    i.qty = cart.First(c => c.ProductId == i.Id).Quantity;
                    i.Description = i.Description.Substring(0, 10);
                }

                return Ok(productdto);
            }
        }


        [HttpPost("checkout")]
        //[Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<IActionResult> CheckOut([FromBody] OrderInformation orderInformation)
        {
            //取得使用者userId
            var userId = HttpContext.User.FindFirstValue("userId");
            int UserID = int.Parse(userId);

            var dict = from li in orderInformation.CheckOutList
                       select new { li.Id, li.Qty };

            //創造訂單詳情集合
            List<Orderdetail> orderdetails = new List<Orderdetail>();
            decimal totalPrice = 0;
            //將dict迴圈取出 製作新的訂單詳情 最後存入我們創造的訂單詳情集合
            foreach (var items in dict)
            {
                var product = await _productRepository.GetProductAsync(items.Id);
                Orderdetail orderdetail = new Orderdetail()
                {
                    ProductId = items.Id,
                    Quantity = items.Qty,
                    UnitPrice = product.OriginalPrice,
                    DiscountPersent = product.DiscountPersent

                };
                orderdetails.Add(orderdetail);
                totalPrice += product.OriginalPrice * (decimal)items.Qty * (decimal)product.DiscountPersent;
            }

            //創造訂單
            Order order = new Order()
            {
                Name = orderInformation.UserInformation.Name,
                Phone = orderInformation.UserInformation.Phone,
                Email = orderInformation.UserInformation.Email,
                OrderStatus = Models.Enum.OrderStatus.NotPaid,
                Date = DateTime.UtcNow,
                Discount = null,
                UserId = UserID,
                Orderdetails = orderdetails
            };

            await _productRepository.AddOrder(order);
            await _productRepository.SaveAsync();


            //向Session取得內容
            List<AddCartItemDto> cart = SessionHelper.GetObjectFromJson<List<AddCartItemDto>>(HttpContext.Session, "cart");

            //查詢要刪除的商品在List裡面的哪一個位置
            foreach (var i in orderInformation.CheckOutList)
            {
                int index = cart.FindIndex(x => x.ProductId == i.Id);
                cart.RemoveAt(index);

                if (cart.Count() < 1)
                {
                    SessionHelper.Remove(HttpContext.Session, "cart");
                }
                else //如果還有商品就重新設定Session內容 把更新後的內容設定上去
                {
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                }
            }

            //取得訂單編號以及訂單總金額
            var orderId = order.Id;
            //Console.WriteLine(orderId);
            //var =await _productRepository.GetOrderdetailByOrderId(orderId);

            var amount = (int)totalPrice;
            //Console.WriteLine(amount);

            string ordernumber = orderId.ToString();

            //var x = productCheck.PayMethod;
            //Console.WriteLine("付款方式"+x);

            var checkOrderInfoData = new checkOrderInfoDto()
            {
                ordernumber = ordernumber,
                amount = amount,
                PayMethod = orderInformation.UserInformation.PayMethod
            };

            return Ok(checkOrderInfoData);
            //return NoContent();

        }
        //[HttpPost("addcart")]
        //[Authorize(AuthenticationSchemes = "Cookies")]
        //public async Task<IActionResult> AddCart([FromBody] AddCartItemDto addShoppingCartItemDto)
        //{
        //    //透過參數獲得特定商品詳細資料
        //    var productFromRepo = await _productRepository.GetProductAsync(addShoppingCartItemDto.ProductId);
        //    var productFromRepoWithNoPicture = await _productRepository.GetProductWithNoPicturesAsync(addShoppingCartItemDto.ProductId);
        //    if(productFromRepo == null)
        //    {
        //        return NotFound("沒有此商品");
        //    }
        //    var productDto = _mapper.Map<ProductDto>(productFromRepo);
        //    //創造購物車裡面的購物內容
        //    var cartitem = new CartItem()
        //    {
        //        Product = productDto,
        //        ProductId = productFromRepo.Id,
        //        OriginalPrice = productFromRepo.OriginalPrice,
        //        DiscountPersent = productFromRepo.DiscountPersent,
        //        Quantity = addShoppingCartItemDto.Quantity
        //    };
        //    //判斷Session內有沒有購物車
        //    if (SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") == null)
        //    {
        //        //如果沒有已存在購物車，則建立一個購物車給使用者
        //        List<CartItem> cart = new List<CartItem>();
        //        cart.Add(cartitem);
        //        SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
        //        foreach(var i in cart)
        //        {
        //            Console.WriteLine(i.ProductId);
        //        }
        //        return Ok(cart);
        //    }
        //    else
        //    {
        //        //如果已經存在購物車 就直接把新的item加上去
        //        //先檢查有沒有相同商品 如果有相同商品則只修改數量
        //        List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
        //        int index = cart.FindIndex(x => x.ProductId == addShoppingCartItemDto.ProductId);
        //        if(index != -1)
        //        {
        //            cart[index].Quantity += cartitem.Quantity;
        //            Console.WriteLine("購物車2:" + cart);
        //        }
        //        else
        //        {
        //            cart.Add(cartitem);
        //            Console.WriteLine("購物車3:" + cart);
        //        }
        //        return Ok(cart);
        //    }
        //}

        //[HttpPost("removecart")]
        //[Authorize(AuthenticationSchemes = "Cookies")]
        //public IActionResult RemoveCart([FromBody] RemoveCartItemDto removeCartItemDto)
        //{
        //    //向Session取得內容
        //    List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

        //    //查詢要刪除的商品在List裡面的哪一個位置
        //    int index = cart.FindIndex(x => x.ProductId == removeCartItemDto.ProductId);
        //    cart.RemoveAt(index);

        //    //檢查購物車內是否還有商品
        //    //如果沒有商品就清除Session
        //    if (cart.Count() < 1)
        //    {
        //        SessionHelper.Remove(HttpContext.Session, "cart");
        //    }
        //    else //如果還有商品就重新設定Session內容 把更新後的內容設定上去
        //    {
        //        SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
        //    }

        //    return NoContent();
        //}

        //[HttpGet]
        //[Authorize(AuthenticationSchemes ="Cookies")]
        //public IActionResult GetCart()
        //{
        //    var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
        //    if(cart == null || cart.Count() == 0 )
        //    {
        //        return Ok("目前購物車尚無商品");
        //    }
        //    else
        //    {
        //        return Ok(cart);
        //    }

        //}

        //[HttpPost("checkout")]
        //[Authorize(AuthenticationSchemes ="Cookies")]
        //public async Task<IActionResult> CheckOut(CartCheck cartCheck)
        //{
        //    //取出使用者Session內容 映射為OrderDetail
        //    var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
        //    var cartDto = _mapper.Map<List<CartItemDto>>(cart);
        //    var cartToOrderDetail = _mapper.Map<List<Orderdetail>>(cartDto);

        //    //取得使用者userId
        //    var userId = HttpContext.User.FindFirstValue("userId");
        //    int UserID = int.Parse(userId);

        //    //創建訂單
        //    var orderDto = new OrderDto()
        //    {
        //        UserId = UserID,
        //        Name = cartCheck.Name,
        //        OrderStatus = Models.Enum.OrderStatus.NotPaid,
        //        Discount = null,
        //        Date = DateTime.UtcNow,
        //        Orderdetails = cartToOrderDetail
        //    };

        //    var order = _mapper.Map<Order>(orderDto);

        //    await _productRepository.AddOrder(order);
        //    await _productRepository.SaveAsync();

        //    return NoContent();

        //}
    }
}
