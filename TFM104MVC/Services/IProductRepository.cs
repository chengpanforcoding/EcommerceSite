using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;
using TFM104MVC.Models.Enum;
using TFM104MVC.ResouceParameters;

namespace TFM104MVC.Services
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync(string keyword,string operatorType,int ratingValue,string Region,string Traveldays , string Triptype,int pageSize,int pageNumber,string OrderBy,string OrderByDesc,string goTouristTime,string product,List<string> Regions);//取得所有商品
        Task<Product> GetProductWithNoPicturesAsync(Guid ProductId);
        Task<Product> GetProductAsync(Guid ProductId);//取得單一商品(使用者輸入商品ID)

        Task<bool> ProductExistAsync(Guid ProductId);
        Task<IEnumerable<ProductPicture>> GetPicturesByProductIdAsync(Guid productId); //主要是透過商品 一起撈出它的子資源圖片
                                                                                   //所以用父資源的Id去判斷(在子資源裡 會有多個父資源Id)
        Task<ProductPicture> GetPictureAsync(int pictureId);

        void AddProduct(Product product);

        Task<bool> SaveAsync();

        void AddProductPicture(Guid productId, ProductPicture productPicture);
        void DeleteProduct(Product product);
        void DeleteProductPicture(ProductPicture productPicture);

        Task AddOrder(Order order);
        Task<IEnumerable<Order>> GetOrders(int userId);
        Task<Orderdetail> GetOrderdetailByProductIdAndOrderId(Guid productId,int orderId);

        string GetProductTitle(Guid id);
        Task<List<Product>> GetProductsByIds(Guid[] productId);

        Task<IEnumerable<Order>> GetAllOrders(string Status,string Keyword); //管理者 廠商 取得所有客戶的訂單
        Task<Order> GetOrderById(int orderId);
        Task<List<Orderdetail>> GetOrderdetailTotalPrice(int orderId);
        Task<List<Order>> GetOrderContentById(int orderId);
        public CountAndPrice OrderTotalCountAndPrice(DateTime sinceTime, DateTime finishTime);
        public IEnumerable<Product> GetNewestProducts(int pencount);
        Task<IEnumerable<Product>> GetProductsForFirmAdminAsync(string keyword, string operatorType, int ratingValue, string region, string travelDays, string tripType, int pageSize, int pageNumber, string orderBy, string orderByDesc, string goTouristTime, string productStatus);
    }
}
