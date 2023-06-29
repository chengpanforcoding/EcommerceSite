using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Models.Entity;
using TFM104MVC.Models.Session;

namespace TFM104MVC.Profiles
{
    public class OrderDetailProfile:Profile
    {
        public OrderDetailProfile()
        {
            //CreateMap<CartItem, Orderdetail>()
            //    .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.OriginalPrice));
            //CreateMap<CartItem, CartItemDto>();
            //CreateMap<CartItemDto, Orderdetail>()
            //    .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.OriginalPrice));
            CreateMap<Orderdetail, OrderdetailDto>();
            CreateMap<Orderdetail, OrderdetailTotalPirceDto>()
                .ForMember(des => des.TotalPrice, opt => opt.MapFrom(src => (decimal)src.DiscountPersent * (decimal)src.Quantity * src.UnitPrice));
        }
    }
}
