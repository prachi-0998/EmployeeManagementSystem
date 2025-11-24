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
    public class DepartmentsController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        

        public DepartmentsController(EMSDbContext dbContext)
        {
            this.dbContext = dbContext;
         
        }

        // GET: api/departments
        [HttpGet]
        public async Task<IActionResult> GetAllDepartmentsAsync()
        {
            var departments = await dbContext.Departments.ToListAsync();

            var departmentsDto = departments.Select(d => new DepartmentsDTO
            {
                DepartmentID = d.DepartmentID,
                DepartmentName = d.DepartmentName,
                IsActive = d.IsActive
            }).ToList();

            return Ok(departmentsDto);
        }

        // GET: api/departments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById([FromRoute] int id)
        {
            var department = dbContext.Departments.FirstOrDefault(d => d.DepartmentID == id);

            if (department == null)
                return NotFound();

            var dto = new DepartmentsDTO
            {
                DepartmentID = department.DepartmentID,
                DepartmentName = department.DepartmentName,
                IsActive = department.IsActive
            };
            return Ok(dto);
        }

        // POST: api/departments
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] AddDepartmentRequestDTO dto)
        {
            var department = new Departments
            {
                DepartmentName = dto.DepartmentName,
                IsActive = true
            };

            dbContext.Departments.Add(department);
            dbContext.SaveChanges();

            var newDto = new DepartmentsDTO
            {
                DepartmentID = department.DepartmentID,
                DepartmentName = department.DepartmentName,
                IsActive = department.IsActive
            };

            return CreatedAtAction(nameof(GetDepartmentById), new { id = newDto.DepartmentID }, newDto);
        }

        // PUT: api/departments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment([FromRoute] int id, [FromBody] UpdateDepartmentRequestDTO dto)
        {
            var department = dbContext.Departments.FirstOrDefault(d => d.DepartmentID == id);
            if (department == null)
                return NotFound();

            department.DepartmentName = dto.DepartmentName;
            department.IsActive = dto.IsActive;

            dbContext.SaveChanges();

            var updatedDto = new DepartmentsDTO
            {
                DepartmentID = department.DepartmentID,
                DepartmentName = department.DepartmentName,
                IsActive = department.IsActive
            };

            return Ok(updatedDto);
        }

        // DELETE: api/departments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment([FromRoute] int id)
        {
            var department = dbContext.Departments.FirstOrDefault(d => d.DepartmentID == id);
            if (department == null)
                return NotFound();

            dbContext.Departments.Remove(department);
            dbContext.SaveChanges();

            return Ok();
        }
    }
}

