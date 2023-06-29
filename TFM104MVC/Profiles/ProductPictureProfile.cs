using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TFM104MVC.Dtos;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Profiles
{
    public class ProductPictureProfile:Profile
    {
        public ProductPictureProfile()
        {
            CreateMap<ProductPicture, ProductPictureDto>();
            CreateMap<ProductPictureForCreationDto, ProductPicture>(); //不用改寫Id是因為ProductPicture的Id屬性為自動編號
                                                                       //與Guid不同 不用去new出一個新的
                                                                       //程式會自動幫我們偵測遞增
            CreateMap<ProductPicture, ProductPictureForCreationDto>();
            CreateMap<ProductPictureDto, ProductPicture>();

        }
    }
}
