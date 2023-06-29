using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Models.Entity;
using TFM104MVC.Models.Session;
using TFM104MVC.ResouceParameters;
using TFM104MVC.Services;

namespace TFM104MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public OrdersController(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpPost("addorder")]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<IActionResult> AddOrder([FromBody] ProductCheck productCheck)
        {
            //1.先取出使用者Id
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            int UserId = int.Parse(userId);
            //使用參數傳進來的ProductId 取得特定商品資料
            var productFromRepo = await _productRepository.GetProductWithNoPicturesAsync(productCheck.ProductId);

            var orderdetail = new Orderdetail()
            {
                UnitPrice = productFromRepo.OriginalPrice,
                ProductId = productFromRepo.Id,
                Quantity = productCheck.Quantity,
                DiscountPersent = productFromRepo.DiscountPersent,
                Product = productFromRepo,
            };

            List<Orderdetail> listOrderDetails = new List<Orderdetail>();
            listOrderDetails.Add(orderdetail);

            var order = new Order()
            {
                Name = productCheck.Name,
                Phone = productCheck.Phone,
                Email = productCheck.Email,
                Discount = null,
                OrderStatus = Models.Enum.OrderStatus.NotPaid,
                Date = DateTime.UtcNow,
                UserId = UserId,
                Orderdetails = listOrderDetails
            };

            await _productRepository.AddOrder(order);
            await _productRepository.SaveAsync();

            //取得訂單編號以及訂單總金額
            var orderId = order.Id;
            //Console.WriteLine(orderId);
            //var =await _productRepository.GetOrderdetailByOrderId(orderId);

            var amt = orderdetail.Quantity * orderdetail.UnitPrice * (decimal)orderdetail.DiscountPersent;
            var amount = (int)amt;
            //Console.WriteLine(amount);

            string ordernumber = orderId.ToString();

            //var x = productCheck.PayMethod;
            //Console.WriteLine("付款方式"+x);

            var checkOrderInfoData = new checkOrderInfoDto()
            {
                ordernumber = ordernumber,
                amount = amount,
                PayMethod = productCheck.PayMethod
            };

            //return RedirectToAction("SpgatewayPayBill", "Bank", new { ordernumber = ordernumber, amount = amount, PayMethod = "creditcard" });

            return Ok(checkOrderInfoData);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<IActionResult> GetOrders() //會員讀取訂單功能
        {
            //1.先取出使用者Id
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            int UserId = int.Parse(userId);

            //2.透過使用者id 叫出特定訂單列表
            var Orders = await _productRepository.GetOrders(UserId);

            if (Orders == null || Orders.Count() == 0)
            {
                return NotFound("目前沒有訂單");
            }

            var orderForShowDto = _mapper.Map<List<OrderForShowDto>>(Orders);
            return Ok(orderForShowDto);
        }

        [HttpGet("{orderId}/{productId}")] // /api/orders/xxx
        [Authorize(AuthenticationSchemes = "Cookies")] //會員讀取單一訂單明細功能
        public async Task<IActionResult> GetOrderdetailByOrderId([FromRoute] int orderId, Guid productId)
        {
            //透過訂單Id 叫出特定訂單詳情(包含訂單 訂單詳情 商品 商品照片)
            var orderdetail = await _productRepository.GetOrderdetailByProductIdAndOrderId(productId, orderId);
            if (orderdetail == null)
            {
                return NotFound("沒有此訂單詳情");
            }
            var orderdetailForShow = _mapper.Map<OrderdetailDto>(orderdetail);

            return Ok(orderdetailForShow);

        }

        [HttpGet("manage")]
        [Authorize(Roles = "Firm,Admin")]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderResourceParameters parameters) //管理者與廠商 讀取訂單 功能
        {
            var allOrders = await _productRepository.GetAllOrders(parameters.Status, parameters.Keyword); //只撈出未付款和已付款的訂單 不撈出已取消的訂單
            if (allOrders == null || allOrders.Count() == 0)
            {
                return NotFound("目前平台沒有此類型訂單");
            }

            var orderForShowDto = _mapper.Map<List<OrderForShowDto>>(allOrders);
            return Ok(orderForShowDto);
        }


        [HttpGet("manage/{orderId}")] //讀取單一訂單 功能
        public async Task<IActionResult> GetOrderById([FromRoute] string orderId)
        {
            int OrderId = int.Parse(orderId);
            var order = await _productRepository.GetOrderContentById(OrderId);
            if (order == null)
            {
                return NotFound("查無此訂單編號");
            }

            var orderForShowDto = _mapper.Map<List<OrderForShowDto>>(order);

            return Ok(orderForShowDto);
        }

        [HttpPost("cancel/{orderId}")]
        //[Authorize(Roles = "Firm,Admin")] //管理者與廠商 軟刪除特定訂單 功能
        public async Task<IActionResult> SoftDeleteOrder([FromRoute] int orderId)
        {
            var order = await _productRepository.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound("查無此訂單編號");
            }

            order.OrderStatus = Models.Enum.OrderStatus.Canceled;

            await _productRepository.SaveAsync();

            return NoContent();
        }

        [HttpPost("done/{orderId}")]
        [Authorize(Roles = "Firm,Admin")] //把訂單狀態轉換成已結案
        public async Task<IActionResult> SoftChangeOrder([FromRoute] int orderId)
        {
            var order = await _productRepository.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound("查無此訂單編號");
            }

            order.OrderStatus = Models.Enum.OrderStatus.Done;

            await _productRepository.SaveAsync();

            return NoContent();
        }

        [HttpPost("gettotalprice/{orderId}")]
        public async Task<IActionResult> GetOrderDetailsTotalPriceByOrderId([FromRoute] int orderId)
        {
            var order = await _productRepository.GetOrderdetailTotalPrice(orderId);
            if (order == null)
            {
                return NotFound("查無此訂單編號");
            }

            var orderDetailsPriceDto = _mapper.Map<List<OrderdetailTotalPirceDto>>(order);
            return Ok(orderDetailsPriceDto);
        }

        [HttpGet("getTodayOrderTotalPriceAndCount")]
        //[Authorize(Roles ="Admin,Firm")]
        public IActionResult GetTodayOrderTotalPriceAndCount() //後台總覽 查看今日訂單筆數與總金額
        {
            DateTime start = Convert.ToDateTime(DateTime.Now.ToString("D"));
            DateTime end = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("D")).AddSeconds(-1);
            var todayOrderCount = _productRepository.OrderTotalCountAndPrice(start, end);
            if (todayOrderCount == null)
            {
                todayOrderCount.Count = 0;
                todayOrderCount.Price = 0;
                return Ok(todayOrderCount);
            }
            return Ok(todayOrderCount);
        }

        [HttpGet("getYesterdayOrderTotalPriceAndCount")]
        public IActionResult GetYesterdayOrderTotalPriceAndCount() //後台總攬 查看昨日訂單筆數與總金額
        {
            DateTime start = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("D"));
            DateTime end = Convert.ToDateTime(DateTime.Now.ToString("D")).AddSeconds(-1);
            var yesterdayOrderCount = _productRepository.OrderTotalCountAndPrice(start, end);
            if (yesterdayOrderCount == null)
            {
                yesterdayOrderCount.Count = 0;
                yesterdayOrderCount.Price = 0;
                return Ok(yesterdayOrderCount);
            }
            return Ok(yesterdayOrderCount);
        }


        [HttpGet("getLastWeekOrderTotalPriceAndCount")]
        public IActionResult GetLastWeekOrderTotalPriceAndCount()
        {
            DateTime start = Convert.ToDateTime(DateTime.Now.AddDays(-7).ToString("D"));
            DateTime end = Convert.ToDateTime(DateTime.Now.ToString("D")).AddSeconds(-1);
            var lastWeekOrderCount = _productRepository.OrderTotalCountAndPrice(start, end);
            if (lastWeekOrderCount == null)
            {
                lastWeekOrderCount.Count = 0;
                lastWeekOrderCount.Price = 0;
                return Ok(lastWeekOrderCount);
            }
            return Ok(lastWeekOrderCount);
        }

        [HttpPost("PayBill")]
        [Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<IActionResult> PayBill([FromBody] BillDto billDto)
        {
            //假裝在處理
            await Task.Delay(3000); //3000毫秒 = 3秒
            //if (returnFault)
            //{
            //    return Ok(new
            //    {
            //        id = Guid.NewGuid(),
            //        created = DateTime.UtcNow,
            //        approved = false,
            //        message = "Reject",
            //        payment_method = "信用卡",
            //        order_number = orderNumber,
            //        card = new
            //        {
            //            card_type = "信用卡",
            //            last_four = "0800"
            //        }
            //    });
            //}

            return Ok(new
            {
                id = Guid.NewGuid(),
                created = DateTime.UtcNow,
                approved = true,
                message = "Approve",
                payment_method = "信用卡",
                order_number = billDto.OrderNumber,
                card = new
                {
                    card_type = "信用卡",
                    last_four = "0800"
                }
            });
        }
    }
}
