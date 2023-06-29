using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TFM104MVC.Dtos;
using TFM104MVC.Models;
using TFM104MVC.Models.Entity;

namespace TFM104MVC.Profiles
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<UserForCreationDto, User>();
            CreateMap<User,UserMemberDto>();
            CreateMap<Member, MemberDto>()
                .ForMember(des => des.Birthday, opt => opt.MapFrom(src => src.Birthday.HasValue ? src.Birthday.Value.ToString("yyyy-MM-dd") : "")) ;
            CreateMap<User, UserFirmDto>();
            CreateMap<Firm, FirmDto>();
        }
    }
}
