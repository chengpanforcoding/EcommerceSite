using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TFM104MVC.Dtos;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;
using TFM104MVC.Models.Enum;
using TFM104MVC.Models.Session;

namespace TFM104MVC.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.OriginalPrice * (decimal)(src.DiscountPersent ?? 1)))
                .ForMember(dest => dest.TravelDays, opt => opt.MapFrom(src => src.TravelDays.ToString()))
                .ForMember(dest => dest.TripType, opt => opt.MapFrom(src => src.TripType.ToString()))
                .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region.ToString()))
                .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => src.ProductStatus.ToString()))
                .ForMember(dest => dest.GoTouristTime, opt => opt.MapFrom(src => src.GoTouristTime.HasValue ? src.GoTouristTime.Value.ToString("yyyy-MM-dd") :""));
         

            CreateMap<ProductCreationDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            CreateMap<ProductUpdateDto, Product>();
                //.ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => (ProductStatus)Enum.Parse(typeof(ProductStatus), src.ProductStatus)))
                //.ForMember(dest => dest.Region, opt => opt.MapFrom(src => (Region)Enum.Parse(typeof(Region), src.Region)))
                //.ForMember(dest => dest.TravelDays, opt => opt.MapFrom(src => (TravelDays)Enum.Parse(typeof(TravelDays), src.TravelDays)));
            CreateMap<Product, ProductUpdateDto>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductForCartDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.OriginalPrice * (decimal)(src.DiscountPersent ?? 1)))
                .ForMember(dest => dest.GoTouristTime, opt => opt.MapFrom(src => src.GoTouristTime.HasValue ? src.GoTouristTime.Value.ToString("yyyy-MM-dd") : ""));
        }
    }
}
