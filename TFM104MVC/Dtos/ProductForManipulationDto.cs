using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.ValidationAttributes;

namespace TFM104MVC.Dtos
{
    [ProductTitleMustBeDifferentFromDescriptionAttribute]
    public abstract class ProductForManipulationDto
    {
        [Required(ErrorMessage = "商品名稱不可為空")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1500)]
        public virtual string Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public double? DiscountPersent { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? GoTouristTime { get; set; }
        public string Notes { get; set; }
        public double? CustomerRating { get; set; }
        public string ProductStatus { get; set; }
        public string TravelDays { get; set; }
        public string TripType { get; set; }
        public string Region { get; set; }

        public ICollection<ProductPictureForCreationDto> ProductPictures { get; set; }
               = new List<ProductPictureForCreationDto>();

        public List<IFormFile> Pic { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(Title == Description)
        //    {
        //        yield return new ValidationResult("產品名稱必須與產品內容不同", new[] { "ProductCreationDto" });
        //    }
        //}
    }
}
