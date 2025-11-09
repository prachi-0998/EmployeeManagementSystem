using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Infra.Data.Context;
using EMS.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        

        public EmployeesController(EMSDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployeesAsync()
        {
            var employees = await dbContext.Employees.ToListAsync();

            var employeesDto = employees.Select(e => new EmployeesDTO
            {
                EmployeeID = e.EmployeeID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                ContactNo = e.ContactNo,
                DepartmentID = e.DepartmentID,
                RoleID = e.RoleID,
                ManagerID = e.ManagerID,
                IsActive = e.IsActive
            }).ToList();

            return Ok(employeesDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployeeById([FromRoute] int id)
        {
            var emp = dbContext.Employees.FirstOrDefault(e => e.EmployeeID == id);
            if (emp == null)
                return NotFound();

            var dto = new EmployeesDTO
            {
                EmployeeID = emp.EmployeeID,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                DepartmentID = emp.DepartmentID,
                RoleID = emp.RoleID
            };
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] AddEmployeeRequestDTO dto)
        {
            var emp = new Employees
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DepartmentID = dto.DepartmentID,
                RoleID = dto.RoleID,
                IsActive = true
            };

            dbContext.Employees.Add(emp);
            dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetEmployeeById), new { id = emp.EmployeeID }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee([FromRoute] int id, [FromBody] UpdateEmployeeRequestDTO dto)
        {
            var emp = dbContext.Employees.FirstOrDefault(e => e.EmployeeID == id);
            if (emp == null)
                return NotFound();

            emp.FirstName = dto.FirstName;
            emp.LastName = dto.LastName;
            emp.DepartmentID = dto.DepartmentID;
            emp.RoleID = dto.RoleID;

            dbContext.SaveChanges();

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee([FromRoute] int id)
        {
            var emp = dbContext.Employees.FirstOrDefault(e => e.EmployeeID == id);
            if (emp == null)
                return NotFound();

            dbContext.Employees.Remove(emp);
            dbContext.SaveChanges();

            return Ok();
        }
    }
}
