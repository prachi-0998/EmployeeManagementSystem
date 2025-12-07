using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.Repository;
using EMS.Infra.Data;
using EMS.Infra.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        private readonly IRoleRepository roleRepository;


        public RolesController(EMSDbContext dbContext, IRoleRepository roleRepository)
        {
            this.dbContext = dbContext;
            this.roleRepository = roleRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<RolesDTO>>> GetAllRolesAsync()
        {
            var roles = await roleRepository.GetAllRolesAsync();

            var rolesDto = new List<RolesDTO>();

            foreach (var role in roles)
            {
                var roleDto = new RolesDTO
                {
                    RoleID = role.RoleID,
                    RoleName = role.RoleName,
                    IsActive = role.IsActive
                };
                rolesDto.Add(roleDto);
            }

            return Ok(rolesDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RolesDTO>> GetRoleByIDAsync([FromRoute] int id)
        {
            var role = await roleRepository.GetRoleByIDAsync(id);

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
        public async Task<ActionResult<AddRoleRequestDTO>> CreateRoleAsync([FromBody] AddRoleRequestDTO dto)
        {
            var roleDomain = new Roles
            {
                RoleName = dto.RoleName
            };


            roleDomain = await roleRepository.CreateRoleAsync(roleDomain);

            //Mapping Domain model back to DTO
            var newRoleDto = new AddRoleRequestDTO
            {
                RoleID = dto.RoleID,
                RoleName = roleDomain.RoleName,
                IsActive = roleDomain.IsActive

            };

            return CreatedAtAction(nameof(GetRoleByIDAsync), new { id = newRoleDto.RoleID }, newRoleDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RolesDTO>> UpdateRoleAsync([FromRoute] int id, [FromBody] UpdateRoleRequestDTO dto)
        {
            var roleDomain = new Roles
            {
                RoleName = dto.RoleName,
            };

            roleDomain = await roleRepository.UpdateRoleAsync(id, roleDomain);

            if (roleDomain == null)
            {
                return NotFound();
            }

            //Convert DomainModel to DTO
            var roleDto = new RolesDTO
            {
                RoleID = roleDomain.RoleID,
                RoleName = roleDomain.RoleName
            };
            return Ok(roleDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<RolesDTO>> DeleteRole([FromRoute] int id)
        {
            var roleDomain = await roleRepository.DeleteRoleAsync(id);
            
            if (roleDomain == null)
            {
                return NotFound();
            }

            var roleDto = new RolesDTO
            {
                RoleID = roleDomain.RoleID,
                RoleName = roleDomain.RoleName

            };
            return Ok(roleDto);
        }
    }
}
