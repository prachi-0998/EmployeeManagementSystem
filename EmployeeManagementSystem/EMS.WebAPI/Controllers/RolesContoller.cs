using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Infra.Data.Context;
using EMS.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
       

        public RolesController(EMSDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            var roles = await dbContext.Roles.ToListAsync();

            var rolesDto = roles.Select(r => new RolesDTO
            {
                RoleID = r.RoleID,
                RoleName = r.RoleName,
                IsActive = r.IsActive
            }).ToList();

            return Ok(rolesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById([FromRoute] int id)
        {
            var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.RoleID == id);
            if (role == null)
                return NotFound();

            var dto = new RolesDTO
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                IsActive = role.IsActive
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] AddRoleRequestDTO dto)
        {
            var role = new Roles
            {
                RoleName = dto.RoleName,
                IsActive = true
            };

            dbContext.Roles.Add(role);
            await dbContext.SaveChangesAsync();

            var newDto = new RolesDTO
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                IsActive = role.IsActive
            };

            return CreatedAtAction(nameof(GetRoleById), new { id = newDto.RoleID }, newDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole([FromRoute] int id, [FromBody] UpdateRoleRequestDTO dto)
        {
            var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.RoleID == id);
            if (role == null)
                return NotFound();

            role.RoleName = dto.RoleName;
            role.IsActive = dto.IsActive;

            await dbContext.SaveChangesAsync();

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole([FromRoute] int id)
        {
            var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.RoleID == id);
            if (role == null)
                return NotFound();

            dbContext.Roles.Remove(role);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
