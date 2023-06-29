using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Services;
using AutoMapper;
using System.Text.RegularExpressions;
using TFM104MVC.ResouceParameters;
using TFM104MVC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Controllers
{
    [Route("api/[controller]")] // api/products
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IProductRepository _productRepository;//調用倉儲
        private readonly IMapper _mapper;//自動映射
        private readonly IWebHostEnvironment _environment;//上傳圖片
        public ProductsController(IProductRepository productRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _environment = webHostEnvironment;
        }

        [HttpGet]
        [HttpHead]  // api/products?Rating=xxx&Region=xxx 
        public async Task<IActionResult> GetProducts(
            [FromQuery] ProductResourceParameters parameters
            //[FromQuery] string keyword,
            //string rating
            )
        {
            Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
            string operatorType = "";
            int ratingValue = -1;
            if (!string.IsNullOrWhiteSpace(parameters.Rating))
            {
                Match match = regex.Match(parameters.Rating);

                if (match.Success)
                {
                    operatorType = match.Groups[1].Value;
                    ratingValue = int.Parse(match.Groups[2].Value);
                }
            }
            var productsFromRepo = await _productRepository.GetProductsAsync(parameters.Keyword, operatorType, ratingValue, parameters.Region, parameters.Traveldays, parameters.Triptype, parameters.PageSize, parameters.PageNumber, parameters.OrderBy, parameters.OrderByDesc, parameters.GoTouristTime,parameters.ProductStatus,parameters.Regions);


            if (productsFromRepo == null || productsFromRepo.Count() <= 0)
            {
                return NotFound("目前沒有商品資料");
            }
            //return Ok(productsFromRepo);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(productsFromRepo);
            return Ok(productsDto);
        }

        // api/products/{productId}
        [HttpGet("{productId}", Name = "GetProductById")] //使用動態路由格式{} 代表路由的最後一格 {productId} 會對應到參數傳進來
        [HttpHead]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var productFromRepo = await _productRepository.GetProductAsync(productId);
            if (productFromRepo == null)
            {
                Console.WriteLine("kello");
                return NotFound($"找不到編號為{productId}的商品");
            }
            //var productDto = new ProductDto()
            //{
            //    Id = productFromRepo.Id,
            //    Title = productFromRepo.Title,
            //    Description = productFromRepo.Description,
            //    Price = productFromRepo.OriginalPrice * (decimal)(productFromRepo.DiscountPersent),
            //    CreateDate = productFromRepo.CreateDate,
            //    UpdateTime = productFromRepo.UpdateTime,
            //    GoTouristTime = productFromRepo.GoTouristTime,
            //    Notes = productFromRepo.Notes,
            //    CustomerRating = productFromRepo.CustomerRating,
            //    TravelDays = productFromRepo.TravelDays.ToString(),
            //    TripType = productFromRepo.TripType.ToString(),
            //    Region = productFromRepo.Region.ToString()
            //};
            var productDto = _mapper.Map<ProductDto>(productFromRepo);
            return Ok(productDto);
        }

        [HttpPost] // api/products
        [Authorize(Roles = "Admin,Firm")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreationDto productCreationDto)
        {
            string rootRoot = _environment.WebRootPath + "/ProductPictures/";
            var productModel = _mapper.Map<Product>(productCreationDto);// 此時Id已被profile檔案投影出一個新的Guid Id
            //var picsize = productCreationDto.Pic.Count;

            var files = productCreationDto.Pic;
            if (files != null)
            {
                foreach (var file in files)
                {
                    ProductPicture productPicture = new ProductPicture();
                    if (file.Length > 0)
                    {
                        var ticks = Guid.NewGuid();
                        using (var stream = System.IO.File.Create(rootRoot + ticks.ToString() + file.FileName))
                        {
                            file.CopyTo(stream);
                        }
                        productPicture.Url = "/ProductPictures/" + ticks.ToString() + file.FileName;
                    }
                    productModel.ProductPictures.Add(productPicture);
                }
            }
            else
            {
                return NotFound("上傳必須要有照片");
            }
            _productRepository.AddProduct(productModel); //這時候只是被寫入數據上下文當中 還沒真正與資料庫互動
            await _productRepository.SaveAsync();
            var productToReturn = _mapper.Map<ProductDto>(productModel);
            return CreatedAtRoute("GetProductById", new { productId = productToReturn.Id }, productToReturn);
        }

        [HttpPut("{productId}")]
        [Authorize(Roles = "Admin,Firm")]
        public async Task<IActionResult> UpdateProduct(
            [FromRoute] Guid productId,
            [FromForm] ProductUpdateDto productUpdateDto
            )
        {
            if (!(await _productRepository.ProductExistAsync(productId)))
            {
                return NotFound("沒有此商品");
            }

            var productFromRepo = await _productRepository.GetProductAsync(productId);
            // 1.把從倉庫提取出來的數據映射為Dto
            // 2.更新這個Dto數據
            // 3.更新完後再映射回原本的Repo
            var productSaveRepo = _mapper.Map(productUpdateDto, productFromRepo);

            string rootRoot = _environment.WebRootPath + "/ProductPictures/";
            var files = productUpdateDto.Pic;
            if (files != null)
            {
                foreach (var file in files)
                {
                    ProductPicture productPicture = new ProductPicture();
                    if (file.Length > 0)
                    {
                        var ticks = Guid.NewGuid();
                        using (var stream = System.IO.File.Create(rootRoot + ticks.ToString() + file.FileName))
                        {
                            file.CopyTo(stream);
                        }
                        productPicture.Url = "/ProductPictures/" + ticks.ToString() + file.FileName;
                    }
                    productSaveRepo.ProductPictures.Add(productPicture);
                }
            }
            var productFromRepoPic = await _productRepository.GetPicturesByProductIdAsync(productId);

            foreach(var pic in productFromRepoPic)
            {
                productSaveRepo.ProductPictures.Add(pic);
            }

            await _productRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{productId}")]
        [Authorize(Roles ="Admin,Firm")]
        public async Task<IActionResult> PartiallyUpdateProfuct(
            [FromRoute] Guid productId,
            [FromBody] JsonPatchDocument<ProductUpdateDto> patchDocument)
        {

            if (!(await _productRepository.ProductExistAsync(productId)))
            {
                return NotFound("此商品不存在");
            }

            //要更新數據 首先要得到此在資料庫裡的數據
            var productFromRepo = await _productRepository.GetProductAsync(productId);
            //得到後 打上補丁
            //JsonPatch一定要對應到參數的類型 所以使用automapper 把原始productFromRepo倒給傳遞進來的ProductUpdateDto類型
            //先去profile文件新增映射關係
            var productToPatch = _mapper.Map<ProductUpdateDto>(productFromRepo);
            //參數對應成功後 把數據打上補丁 使用JsonPatchDocument內建method 代表這個補丁成功打給了productToPatch
            patchDocument.ApplyTo(productToPatch, ModelState);
            if (!TryValidateModel(productToPatch))
            {
                return ValidationProblem(ModelState);
            }
            //最後再把productToPatch倒給product型態的productFromRepo 存入資料庫中
            _mapper.Map(productToPatch, productFromRepo); // .Map(輸入的數據,輸出的數據) 為什麼不是.Map<>() 因為這個是既有的資料更新 不是新增
            await _productRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles = "Admin,Firm")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid productId)
        {
            if (!(await _productRepository.ProductExistAsync(productId)))
            {
                return NotFound("沒有此商品");
            }
            var result = await _productRepository.GetProductAsync(productId);
            _productRepository.DeleteProduct(result);
            await _productRepository.SaveAsync();

            return NoContent();
        }

        [HttpPost("{productId}/soft")]
        [Authorize(Roles ="Admin,Firm")]
        public async Task<IActionResult> SoftDeleteProduct([FromRoute]Guid productId)
        {
            if(!await _productRepository.ProductExistAsync(productId))
            {
                return NotFound("沒有此商品");
            }
            var productFromRepo = await _productRepository.GetProductAsync(productId);
            productFromRepo.ProductStatus = Models.Enum.ProductStatus.NotSold;
            await _productRepository.SaveAsync();

            return Ok("商品已下架");
        }

        [HttpPost("{productId}/goPublic")]
        //[Authorize(Roles = "Admin,Firm")]
        public async Task<IActionResult> GoPublicProduct([FromRoute] Guid productId)
        {
            if (!await _productRepository.ProductExistAsync(productId))
            {
                return NotFound("沒有此商品");
            }
            var productFromRepo = await _productRepository.GetProductAsync(productId);
            productFromRepo.ProductStatus = Models.Enum.ProductStatus.Launched;
            await _productRepository.SaveAsync();

            return Ok("商品已上架");
        }


        [HttpGet("newest")]
        public IActionResult GetNewestProduct()
        {
            var productsFromRepo = _productRepository.GetNewestProducts(4);
            var productsDto = _mapper.Map<List<ProductDto>>(productsFromRepo);

            return Ok(productsDto);
        }
    }
}
