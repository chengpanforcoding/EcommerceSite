﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TFM104MVC.Dtos
{
    public class ProductForCartDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } //計算方式為 OriginalPrice*DiscountPersent 等等在profile文件見真章
                                           //profile文件負責處理映射的改寫 超猛der~
        public string GoTouristTime { get; set; }
        public double? CustomerRating { get; set; }
        public int qty { get; set; }
        public ICollection<ProductPictureDto> ProductPictures { get; set; }
    }
}
