using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Profiles
{
    public class CustomerRatingProfile : Profile
    {
        public CustomerRatingProfile()
        {
            CreateMap<CustomerRating, CustomerRatingDto>();
        }
    }
}
