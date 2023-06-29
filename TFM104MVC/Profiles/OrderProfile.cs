using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderDto, Order>();
            CreateMap<Order, OrderForShowDto>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString()));

        }
    }
}
