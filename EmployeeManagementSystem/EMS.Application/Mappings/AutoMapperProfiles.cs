using AutoMapper;
using EMS.Application.DTO;
using EMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Users, UsersDTO>().ReverseMap();
            CreateMap<AddUserRequestDTO, Users>().ReverseMap();
            CreateMap<UpdateUserRequestDTO, Users>().ReverseMap();
        }
    }
}
