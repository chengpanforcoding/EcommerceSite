using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;
using TFM104MVC.Services;

namespace TFM104MVC.Controllers
{
    [Route("api/products/{productId}/pictures")]
    [ApiController]
    public class ProductPicturesController : ControllerBase
    {
        private IProductRepository _productRepository;
        private IMapper _mapper;
        public ProductPicturesController(IProductRepository productRepository , IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPicturesForProduct(Guid productId)
        {
            if (!(await _productRepository.ProductExistAsync(productId)))
            {
                return NotFound("此商品不存在");
            }
            var productPicturesFromRepo = await _productRepository.GetPicturesByProductIdAsync(productId);
            if(productPicturesFromRepo == null || productPicturesFromRepo.Count() <= 0)
            {
                return NotFound("此商品的照片不存在");
            }
            //return Ok(productPicturesFromRepo);
            return Ok(_mapper.Map<IEnumerable<ProductPictureDto>>(productPicturesFromRepo));
        }

        [HttpGet("{pictureId}",Name = "GetPicture")] // 代表路由為 api/products/{productId}/pictures/{pictureId}
        public async Task<IActionResult> GetPicture(Guid productId ,int pictureId)
        {
            if (!(await _productRepository.ProductExistAsync(productId)))
            {
                return NotFound("此商品不存在");
            }
            var pictureFromRepo = await _productRepository.GetPictureAsync(pictureId);
            if(pictureFromRepo == null)
            {
                return NotFound("此照片不存在");
            }
            //return Ok(pictureFromRepo);
            return Ok(_mapper.Map<ProductPictureDto>(pictureFromRepo));
        }

        [HttpPost] // /api/products/{productId}/pictures
        [Authorize(Roles = "Admin,Firm")]

        public async Task<IActionResult> CreateProductPicture(
            [FromRoute]Guid productId,
            [FromBody] ProductPictureForCreationDto productPictureForCreationDto
            )
        {
            if (!(await _productRepository.ProductExistAsync(productId)))
            {
                return NotFound("此商品不存在");
            }
            var pictureModel = _mapper.Map<ProductPicture>(productPictureForCreationDto);
            _productRepository.AddProductPicture(productId, pictureModel);
            await _productRepository.SaveAsync();
            var pictureToReturn = _mapper.Map<ProductPictureDto>(pictureModel);
            return CreatedAtRoute("GetPicture", new { productId = pictureModel.ProductId, pictureId = pictureModel.Id }, pictureToReturn);
        }
        [HttpDelete("{pictureId}")]
        [Authorize(Roles = "Admin,Firm")]

        public async Task<IActionResult> DeletePicture([FromRoute]Guid productId,[FromRoute] int pictureId)
        {
            if (!(await _productRepository.ProductExistAsync(productId)))
            {
                return NotFound("此商品不存在");
            }
            var result = await _productRepository.GetPictureAsync(pictureId);
            if(result == null)
            {
                return NotFound("此照片不存在");
            }
            _productRepository.DeleteProductPicture(result);
            await _productRepository.SaveAsync();

            return NoContent();
        }
    }
}
