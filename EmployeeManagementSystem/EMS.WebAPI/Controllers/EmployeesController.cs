using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.Repository;
using EMS.Infra.Data;
using EMS.Infra.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        private readonly IEmployeeRepository empRepository;


        public EmployeesController(EMSDbContext dbContext, IEmployeeRepository empRepository)
        {
            this.dbContext = dbContext;
            this.empRepository = empRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeesDTO>>> GetAllEmployeesAsync()
        {
            var empsDomain = await empRepository.GetAllEmployeesAsync();

            var empsDto = new List<EmployeesDTO>();

            foreach (var empDomain in empsDomain)
            {
                var empDto = new EmployeesDTO
                {
                    EmployeeID = empDomain.EmployeeID,
                    FirstName = empDomain.FirstName,
                    LastName = empDomain.LastName,
                    ContactNo = empDomain.ContactNo,
                    DateOfBirth = empDomain.DateOfBirth,
                    DepartmentID = empDomain.DepartmentID,
                    RoleID = empDomain.RoleID,
                    HireDate = empDomain.HireDate,
                    Salary = empDomain.Salary,
                    ManagerID = empDomain.ManagerID,
                    IsActive = empDomain.IsActive,
                    UserID = empDomain.UserID
                };
                empsDto.Add(empDto);
            }

            return Ok(empsDto);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeesDTO>> GetEmployeeByIDAsync([FromRoute] int id)
        {
            var emp = await empRepository.GetEmployeeByIDAsync(id);

            if (emp == null)
                return NotFound();

            var dto = new EmployeesDTO
            {
                EmployeeID = emp.EmployeeID,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                ContactNo = emp.ContactNo,
                DateOfBirth = emp.DateOfBirth,
                DepartmentID = emp.DepartmentID,
                RoleID = emp.RoleID,
                HireDate = emp.HireDate,
                Salary = emp.Salary,
                ManagerID = emp.ManagerID,
                IsActive = emp.IsActive,
                UserID = emp.UserID
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<AddEmployeeRequestDTO>> CreateEmployeeAsync([FromBody] AddEmployeeRequestDTO dto)
        {
            var empDomain = new Employees
            {
                EmployeeID = dto.EmployeeID,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                ContactNo = dto.ContactNo,
                DateOfBirth = dto.DateOfBirth,
                DepartmentID = dto.DepartmentID,
                RoleID = dto.RoleID,
                HireDate = dto.HireDate,
                Salary = dto.Salary,
                ManagerID = dto.ManagerID,
                IsActive = dto.IsActive,
                UserID = dto.UserID
            };


            empDomain = await empRepository.CreateEmployeeAsync(empDomain);

            //Mapping Domain model back to DTO
            var newempDto = new AddEmployeeRequestDTO
            {
                EmployeeID = empDomain.EmployeeID,
                FirstName = empDomain.FirstName,
                LastName = empDomain.LastName,
                ContactNo = empDomain.ContactNo,
                DateOfBirth = empDomain.DateOfBirth,
                DepartmentID = empDomain.DepartmentID,
                RoleID = empDomain.RoleID,
                HireDate = empDomain.HireDate,
                Salary = empDomain.Salary,
                ManagerID = empDomain.ManagerID,
                IsActive = empDomain.IsActive,
                UserID = empDomain.UserID

            };

            return CreatedAtAction(nameof(GetEmployeeByIDAsync), new { id = newempDto.EmployeeID }, newempDto);

            
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeesDTO>> UpdateEmployeeAsync([FromRoute] int id, [FromBody] UpdateEmployeeRequestDTO dto)
        {
            var empDomain = new Employees
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DepartmentID = dto.DepartmentID,
                RoleID = dto.RoleID,
                IsActive = dto.IsActive
            };
            
           ;
            empDomain = await empRepository.UpdateEmployeeAsync(id, empDomain);

            if (empDomain == null)
            {
                return NotFound();
            }

            //Convert DomainModel to DTO
            var empDto = new EmployeesDTO
            {
                FirstName = empDomain.FirstName,
                LastName = empDomain.LastName,
                DepartmentID = empDomain.DepartmentID,
                RoleID = empDomain.RoleID,
                IsActive = empDomain.IsActive
            };
            return Ok(empDto);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<EmployeesDTO>> DeleteEmployeeAsync([FromRoute] int id)
        {
            var empDomain = await empRepository.DeleteEmployeeAsync(id);
            //Check if user exists
            if (empDomain == null)
            {
                return NotFound();
            }

            //returning deleted user back after Converting DomainModel to DTO
            var empDto = new EmployeesDTO
            {
                FirstName = empDomain.FirstName,
                LastName = empDomain.LastName,
                DepartmentID = empDomain.DepartmentID,
                RoleID = empDomain.RoleID

            };
            return Ok(empDto);
        }
    }
}
