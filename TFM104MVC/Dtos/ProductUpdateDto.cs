using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.ValidationAttributes;

namespace TFM104MVC.Dtos
{

    //[ProductTitleMustBeDifferentFromDescriptionAttribute]
    public class ProductUpdateDto:ProductForManipulationDto
    {
        [Required(ErrorMessage ="更新必備")]
        [MaxLength(1500)]
        public override string Description { get; set; }
    }
}
