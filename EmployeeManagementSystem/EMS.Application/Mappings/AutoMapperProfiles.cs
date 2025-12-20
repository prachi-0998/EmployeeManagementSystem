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
            //Users mappings
            CreateMap<Users, UsersDTO>().ReverseMap();
            CreateMap<AddUserRequestDTO, Users>().ReverseMap();
            CreateMap<UpdateUserRequestDTO, Users>().ReverseMap();

            // Departments mappings
            CreateMap<Departments, DepartmentsDTO>().ReverseMap();
            CreateMap<AddDepartmentRequestDTO, Departments>().ReverseMap();
            CreateMap<UpdateDepartmentRequestDTO, Departments>().ReverseMap();

            // Employees mappings
            CreateMap<Employees, EmployeesDTO>().ReverseMap();
            CreateMap<AddEmployeeRequestDTO, Employees>().ReverseMap();
            CreateMap<UpdateEmployeeRequestDTO, Employees>().ReverseMap();

            // Roles mappings
            CreateMap<Roles, RolesDTO>().ReverseMap();
            CreateMap<AddRoleRequestDTO, Roles>().ReverseMap();
            CreateMap<UpdateRoleRequestDTO, Roles>().ReverseMap();

            // UserRole mappings
            CreateMap<UserRole, UserRoleDTO>().ReverseMap();
            CreateMap<AddUserRoleRequestDTO, UserRole>().ReverseMap();
            CreateMap<UpdateUserRoleRequestDTO, UserRole>().ReverseMap();
        }
    }
}
