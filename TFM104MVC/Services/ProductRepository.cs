using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TFM104MVC.Database;
using TFM104MVC.Dtos;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;
using TFM104MVC.Models.Enum;
using TFM104MVC.ResouceParameters;

namespace TFM104MVC.Services
{
    public class ProductRepository : IProductRepository
    {
        private AppDbContext _context;
        private IAuthenticateRepository _authenticateRepository;
        private IHttpContextAccessor _httpContextAccessor;
        public ProductRepository(AppDbContext context , IAuthenticateRepository authenticateRepository, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _authenticateRepository = authenticateRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Product> GetProductAsync(Guid ProductId)
        {          
            return await _context.Products.Include(t=>t.ProductPictures).FirstOrDefaultAsync(n => n.Id == ProductId);
            //return _context.Products.Where(n => n.Id == ProductId).FirstOrDefault();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(string keyword , string operatorType , int ratingValue,string region,string travelDays,string tripType,int pageSize, int pageNumber,string orderBy,string orderByDesc,string goTouristTime,string productStatus,List<string> Regions)
        {
            var userIdStr = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
            int userId = 1;
            if (!string.IsNullOrEmpty(userIdStr))
            {
                userId = int.Parse(userIdStr);
            }
            var user = _authenticateRepository.FindTheOnlyUser(userId);

            IQueryable<Product> result = _context.Products.Include(t => t.ProductPictures);

            if (user == null || user.RoleName == null || user.RoleName == "Member")
            {
                result = result.Where(x => x.ProductStatus == ProductStatus.Launched || x.ProductStatus == ProductStatus.Unapproved);
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(n => n.Title.Contains(keyword) || n.Description.Contains(keyword)||n.OriginalPrice.ToString().Contains(keyword));
            }
            if (ratingValue > 0)
            {
                switch (operatorType)
                {
                    case "largerThan":
                        result = result.Where(t => t.CustomerRating >= ratingValue);
                        break;
                    case "lessThan":
                        result = result.Where(t => t.CustomerRating <= ratingValue);
                        break;
                    case "equalTo":
                        result = result.Where(t => t.CustomerRating == ratingValue);
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(region))
            {
                region = region.Trim();
                var r1 = (Region)Enum.Parse(typeof(Region), region);
                result = result.Where(n => n.Region == r1);
            }
            if (!string.IsNullOrWhiteSpace(travelDays))
            {
                travelDays = travelDays.Trim();
                var r2 = (TravelDays)Enum.Parse(typeof(TravelDays), travelDays);
                result = result.Where(n => n.TravelDays == r2);
            }
            if (!string.IsNullOrWhiteSpace(tripType))
            {
                tripType = tripType.Trim();
                var r3 = (TripType)Enum.Parse(typeof(TripType), tripType);
                result = result.Where(n => n.TripType == r3);
            }
            if (!string.IsNullOrWhiteSpace(goTouristTime))
            {
                DateTime? a = Convert.ToDateTime(goTouristTime);
                string b = a.HasValue ? a.Value.ToString("yyyy-MM-dd") : "";
                var c = DateTime.Parse(b);
                result = result.Where(n => n.GoTouristTime == c);
            }
            if (!string.IsNullOrWhiteSpace(productStatus))
            {
                productStatus = productStatus.Trim();
                var r4 = (ProductStatus)Enum.Parse(typeof(ProductStatus), productStatus);
                result = result.Where(n => n.ProductStatus == r4);
            }
            ////分頁功能的實現放在最後 因為首先要過濾數據 搜索排序 最後再形成分頁
            ////分頁思路
            ////跳過一定量的資料(例如 使用者要到第七頁 代表數據要跳過前六頁的所有內容=> (頁數-1)*大小)
            //var skip = (pageNumber - 1) * pageSize;
            //result = result.Skip(skip);

            ////以pageSize為標準顯示一定量的資料
            //result = result.Take(pageSize);

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy.ToLowerInvariant())
                {
                    case "originalprice":
                        result = result.OrderBy(x => x.OriginalPrice*(decimal)(x.DiscountPersent ?? 1 ));
                        break;
                    case "createdate":
                        result = result.OrderBy(x => x.CreateDate);
                        break;
                    case "customerrating":
                        result = result.OrderBy(x => x.CustomerRating);
                        break;
                    case "gotouristtime":
                        result = result.OrderBy(x => x.GoTouristTime);
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(orderByDesc))
            {
                switch (orderByDesc.ToLowerInvariant())
                {
                    case "originalprice":
                        result = result.OrderByDescending(x => x.OriginalPrice*(decimal)(x.DiscountPersent ?? 1));
                        break;
                    case "createdate":
                        result = result.OrderByDescending(x => x.CreateDate);
                        break;
                    case "customerrating":
                        result = result.OrderByDescending(x => x.CustomerRating);
                        break;
                    case "gotouristtime":
                        result = result.OrderByDescending(x => x.GoTouristTime);
                        break;
                }
            }
            List<Region?> r = new List<Region?>();
            foreach(var i in Regions)
            {
                if (i != null)
                {
                    var a = (Region)Enum.Parse(typeof(Region), i);
                    r.Add(a);
                }
            }
            if (r.Any())
            {
                result = result.Where(x => r.Contains(x.Region));
            }
            return await result.ToListAsync();
        }

        public async Task<IEnumerable<ProductPicture>> GetPicturesByProductIdAsync(Guid productId)
        {
            return await _context.ProductPictures.Where(n => n.ProductId == productId).ToListAsync();
        }

        public async Task<bool> ProductExistAsync(Guid ProductId)
        {
            //var result = _context.Products.Where(n => n.Id == ProductId).FirstOrDefault();
            //if (result == null)
            //{
            //    return false;
            //}
            //return true;
            return await _context.Products.AnyAsync(n => n.Id == ProductId);

            //兩種寫法 第一種會出現序列化例外??
        }

        public async Task<ProductPicture> GetPictureAsync(int pictureId)
        {
            return await _context.ProductPictures.Where(n => n.Id == pictureId).FirstOrDefaultAsync();
        }

        public void AddProduct(Product product)
        {
            if(product == null)
            {
                throw new ArgumentNullException(nameof(product)); //代表 若Body數據裡面沒東西 則拋出例外
            }
            _context.Products.Add(product);
            //_context.SaveChanges();
        }

        public async Task<bool> SaveAsync()
        {
            return  (await _context.SaveChangesAsync() >= 0); //如果小於0 代表保存失敗
        }

        public void AddProductPicture(Guid productId, ProductPicture productPicture)
        {
            if(productId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(productId));
            }
            if(productPicture == null)
            {
                throw new ArgumentNullException(nameof(productPicture));
            }
            productPicture.ProductId = productId;
            _context.ProductPictures.Add(productPicture);
        }

        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
        }

        public void DeleteProductPicture(ProductPicture productPicture)
        {
            _context.ProductPictures.Remove(productPicture);
        }

        public async Task<Product> GetProductWithNoPicturesAsync(Guid ProductId)
        {
            return await _context.Products.Where(n => n.Id == ProductId).FirstOrDefaultAsync();
        }


        public async Task AddOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<IEnumerable<Order>> GetOrders(int userId)
        {
            //return await _context.Orders.Include(x=>x.Orderdetails).Where(x => x.UserId == userId).ToListAsync();
            return await _context.Orders.Include(x => x.Orderdetails).ThenInclude(x=>x.Product).ThenInclude(x=>x.ProductPictures).Where(x => x.UserId == userId).OrderByDescending(x => x.Date).ToListAsync();
        }

        public async Task<Orderdetail> GetOrderdetailByProductIdAndOrderId(Guid productId,int orderId)
        {
            return await _context.Orderdetails.Include(x => x.Product).ThenInclude(x => x.ProductPictures).Where(x => x.ProductId == productId && x.OrderId == orderId).FirstOrDefaultAsync();
        }

        public string GetProductTitle(Guid id)
        {
            return _context.Products.FirstOrDefault(x => x.Id == id)?.Title;
        }

        public async Task<List<Product>> GetProductsByIds(Guid[] productId)
        {
            return await _context.Products.Include(x=>x.ProductPictures).Where(x => productId.Contains(x.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrders(string Status,string Keyword)
        {
            IQueryable<Order> result = _context.Orders.Include(x => x.Orderdetails).ThenInclude(x => x.Product).ThenInclude(x => x.ProductPictures);
            if (!string.IsNullOrWhiteSpace(Status))
            {
                Status = Status.Trim();
                var r1 = (OrderStatus)Enum.Parse(typeof(OrderStatus), Status);
                result = result.Where(x => x.OrderStatus == r1);
            }
            if (!string.IsNullOrWhiteSpace(Keyword))
            {
                Keyword = Keyword.Trim();
                result = result.Where(x => x.Name.Contains(Keyword) || x.Id.ToString().Contains(Keyword));
            }
            result = result.OrderByDescending(x => x.Date);
            return await result.ToListAsync();
            //return await _context.Orders.Include(x => x.Orderdetails).ThenInclude(x => x.Product).ThenInclude(x => x.ProductPictures).Where(x=>x.OrderStatus == OrderStatus.NotPaid || x.OrderStatus == OrderStatus.Paid).ToListAsync();
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        }

        public async Task<List<Orderdetail>> GetOrderdetailTotalPrice(int orderId)
        {
            return await _context.Orderdetails.Where(x => x.OrderId == orderId).ToListAsync();
        }

        public async Task<List<Order>> GetOrderContentById(int orderId)
        {
            return await _context.Orders.Include(x => x.Orderdetails).ThenInclude(x => x.Product).Where(x => x.Id == orderId).ToListAsync();
        }

        public CountAndPrice OrderTotalCountAndPrice(DateTime sinceTime,DateTime finishTime)
        {
            var count = _context.Orders.Where(x => x.Date.CompareTo(sinceTime)>=0 && x.Date.CompareTo(finishTime)<=0).Count();
            var price = _context.Orderdetails.Where(x => x.Order.Date >= sinceTime && x.Order.Date <= finishTime).Sum(x => (decimal)x.Quantity * x.UnitPrice * (decimal)x.DiscountPersent);
            CountAndPrice result = new CountAndPrice()
            {
                Count = count,
                Price = price
            };
            return result;
        }

        public IEnumerable<Product> GetNewestProducts(int pencount)
        {
            int count = _context.Products.Count();
            var result = _context.Products.Include(x => x.ProductPictures).Skip(count- pencount);
            return result.ToList();
        }

        public async Task<IEnumerable<Product>> GetProductsForFirmAdminAsync(string keyword, string operatorType, int ratingValue, string region, string travelDays, string tripType, int pageSize, int pageNumber, string orderBy, string orderByDesc, string goTouristTime, string productStatus)
        {
            IQueryable<Product> result = _context.Products.Include(t => t.ProductPictures);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(n => n.Title.Contains(keyword) || n.Description.Contains(keyword) || n.OriginalPrice.ToString().Contains(keyword));
            }
            if (ratingValue > 0)
            {
                switch (operatorType)
                {
                    case "largerThan":
                        result = result.Where(t => t.CustomerRating >= ratingValue);
                        break;
                    case "lessThan":
                        result = result.Where(t => t.CustomerRating <= ratingValue);
                        break;
                    case "equalTo":
                        result = result.Where(t => t.CustomerRating == ratingValue);
                        break;
                }
            }
            //switch (region.)
            //{
            //    case 
            //}
            if (!string.IsNullOrWhiteSpace(region))
            {
                region = region.Trim();
                var r1 = (Region)Enum.Parse(typeof(Region), region);
                result = result.Where(n => n.Region == r1);
            }
            if (!string.IsNullOrWhiteSpace(travelDays))
            {
                travelDays = travelDays.Trim();
                var r2 = (TravelDays)Enum.Parse(typeof(TravelDays), travelDays);
                result = result.Where(n => n.TravelDays == r2);
            }
            if (!string.IsNullOrWhiteSpace(tripType))
            {
                tripType = tripType.Trim();
                var r3 = (TripType)Enum.Parse(typeof(TripType), tripType);
                result = result.Where(n => n.TripType == r3);
            }
            if (!string.IsNullOrWhiteSpace(goTouristTime))
            {
                DateTime? a = Convert.ToDateTime(goTouristTime);
                string b = a.HasValue ? a.Value.ToString("yyyy-MM-dd") : "";
                var c = DateTime.Parse(b);
                result = result.Where(n => n.GoTouristTime == c);
            }
            if (!string.IsNullOrWhiteSpace(productStatus))
            {
                productStatus = productStatus.Trim();
                var r4 = (ProductStatus)Enum.Parse(typeof(ProductStatus), productStatus);
                result = result.Where(n => n.ProductStatus == r4);
            }
            ////分頁功能的實現放在最後 因為首先要過濾數據 搜索排序 最後再形成分頁
            ////分頁思路
            ////跳過一定量的資料(例如 使用者要到第七頁 代表數據要跳過前六頁的所有內容=> (頁數-1)*大小)
            //var skip = (pageNumber - 1) * pageSize;
            //result = result.Skip(skip);

            ////以pageSize為標準顯示一定量的資料
            //result = result.Take(pageSize);

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy.ToLowerInvariant())
                {
                    case "originalprice":
                        result = result.OrderBy(x => x.OriginalPrice * (decimal)(x.DiscountPersent ?? 1));
                        break;
                    case "createdate":
                        result = result.OrderBy(x => x.CreateDate);
                        break;
                    case "customerrating":
                        result = result.OrderBy(x => x.CustomerRating);
                        break;
                    case "gotouristtime":
                        result = result.OrderBy(x => x.GoTouristTime);
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(orderByDesc))
            {
                switch (orderByDesc.ToLowerInvariant())
                {
                    case "originalprice":
                        result = result.OrderByDescending(x => x.OriginalPrice * (decimal)(x.DiscountPersent ?? 1));
                        break;
                    case "createdate":
                        result = result.OrderByDescending(x => x.CreateDate);
                        break;
                    case "customerrating":
                        result = result.OrderByDescending(x => x.CustomerRating);
                        break;
                    case "gotouristtime":
                        result = result.OrderByDescending(x => x.GoTouristTime);
                        break;
                }
            }

            return await result.ToListAsync();
        }
    }
}
