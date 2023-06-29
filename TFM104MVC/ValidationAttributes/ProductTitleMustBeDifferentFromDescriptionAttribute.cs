using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Dtos;

namespace TFM104MVC.ValidationAttributes
{
    public class ProductTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var productDto = (ProductForManipulationDto)validationContext.ObjectInstance;
            if (productDto.Title == productDto.Description)
            {
                return new ValidationResult("產品名稱必須與產品描述不同", new[] { "ProductCreationDto" });
            }
            return ValidationResult.Success;
        }
    }
}
