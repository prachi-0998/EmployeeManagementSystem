using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.Repository;
using EMS.Infra.Data;
using EMS.Infra.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        private readonly IDepartmentRepository deptRepository;


        public DepartmentsController(EMSDbContext dbContext, IDepartmentRepository deptRepository)
        {
            this.dbContext = dbContext;
            this.deptRepository = deptRepository;

        }

        // GET: api/departments
        [HttpGet]
        public async Task<IActionResult> GetAllDepartmentsAsync()
        {
            var deptsDomain = await deptRepository.GetAllDepartmentsAsync();

            var deptsDto = new List<DepartmentsDTO>();

            foreach (var deptDomain in deptsDomain)
            {
                var deptDto = new DepartmentsDTO
                {
                    DepartmentID = deptDomain.DepartmentID,
                    DepartmentName = deptDomain.DepartmentName,
                    IsActive = deptDomain.IsActive
                };
                deptsDto.Add(deptDto);
            }
            
            return Ok(deptsDto);
        }

        // GET: api/departments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentByIDAsync([FromRoute] int id)
        {
            var department = await deptRepository.GetDepartmentByIDAsync(id);

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
        public async Task<IActionResult> CreateDepartmentAsync([FromBody] AddDepartmentRequestDTO dto)
        {
           
            var deptDomain = new Departments
            {
                DepartmentName = dto.DepartmentName
            };

          
            deptDomain = await deptRepository.CreateDepartmentAsync(deptDomain);

            //Mapping Domain model back to DTO
            var newuserDto = new AddDepartmentRequestDTO
            {
                DepartmentID = deptDomain.DepartmentID,
                DepartmentName = deptDomain.DepartmentName,
                
            };

            return CreatedAtAction(nameof(GetDepartmentByIDAsync), new { id = newuserDto.DepartmentID }, newuserDto);
        }

        // PUT: api/departments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartmentAsync([FromRoute] int id, [FromBody] UpdateDepartmentRequestDTO dto)
        {
            var deptDomain = new Departments
            {
                DepartmentName = dto.DepartmentName,
            };
            //var userDomain = await dbContext.Users.FirstOrDefaultAsync(u => u.UserID == id);
            deptDomain = await deptRepository.UpdateDepartmentAsync(id, deptDomain);

            if (deptDomain == null)
            {
                return NotFound();
            }

            //Convert DomainModel to DTO
            var deptDto = new DepartmentsDTO
            {
                DepartmentID = deptDomain.DepartmentID,
                DepartmentName = deptDomain.DepartmentName
            };
            return Ok(deptDto);

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
        public async Task<IActionResult> DeleteDepartmentAsync([FromRoute] int id)
        {
            var deptDomain = await deptRepository.DeleteDepartmentAsync(id);
            //Check if user exists
            if (deptDomain == null)
            {
                return NotFound();
            }

            //returning deleted user back after Converting DomainModel to DTO
            var deptDto = new DepartmentsDTO
            {
                DepartmentID = deptDomain.DepartmentID,
                DepartmentName = deptDomain.DepartmentName
               
            };
            return Ok(deptDto);
        }
    }
}

