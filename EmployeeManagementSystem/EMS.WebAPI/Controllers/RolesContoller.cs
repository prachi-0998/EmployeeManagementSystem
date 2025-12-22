using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.Exceptions;
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
        private readonly ILogger<RolesController> _logger;


        public RolesController(EMSDbContext dbContext, IRoleRepository roleRepository, ILogger<RolesController> logger)
        {
            this.dbContext = dbContext;
            this.roleRepository = roleRepository;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<RolesDTO>>> GetAllRolesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all roles");
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

                _logger.LogInformation("Successfully retrieved {Count} roles", rolesDto.Count);
                return Ok(rolesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all roles");
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RolesDTO>> GetRoleByIDAsync([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Fetching role with ID: {RoleId}", id);
                var role = await roleRepository.GetRoleByIDAsync(id);

                if (role == null)
                {
                    _logger.LogWarning("Role with ID: {RoleId} not found", id);
                    throw new NotFoundException("Role", id);
                }

                var dto = new RolesDTO
                {
                    RoleID = role.RoleID,
                    RoleName = role.RoleName,
                    IsActive = role.IsActive
                };

                _logger.LogInformation("Successfully retrieved role with ID: {RoleId}", id);
                return Ok(dto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching role with ID: {RoleId}", id);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<AddRoleRequestDTO>> CreateRoleAsync([FromBody] AddRoleRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for creating role");
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Creating new role: {RoleName}", dto.RoleName);
                var roleDomain = new Roles
                {
                    RoleName = dto.RoleName
                };

                roleDomain = await roleRepository.CreateRoleAsync(roleDomain);

                var newRoleDto = new AddRoleRequestDTO
                {
                    RoleID = roleDomain.RoleID,
                    RoleName = roleDomain.RoleName,
                    IsActive = roleDomain.IsActive
                };

                _logger.LogInformation("Successfully created role with ID: {RoleId}", newRoleDto.RoleID);
                return Created($"/api/Roles/{newRoleDto.RoleID}", newRoleDto);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating role");
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RolesDTO>> UpdateRoleAsync([FromRoute] int id, [FromBody] UpdateRoleRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for updating role with ID: {RoleId}", id);
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Updating role with ID: {RoleId}", id);
                var roleDomain = new Roles
                {
                    RoleName = dto.RoleName,
                };

                roleDomain = await roleRepository.UpdateRoleAsync(id, roleDomain);

                if (roleDomain == null)
                {
                    _logger.LogWarning("Role with ID: {RoleId} not found for update", id);
                    throw new NotFoundException("Role", id);
                }

                var roleDto = new RolesDTO
                {
                    RoleID = roleDomain.RoleID,
                    RoleName = roleDomain.RoleName
                };

                _logger.LogInformation("Successfully updated role with ID: {RoleId}", id);
                return Ok(roleDto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating role with ID: {RoleId}", id);
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<RolesDTO>> DeleteRole([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Deleting role with ID: {RoleId}", id);
                var roleDomain = await roleRepository.DeleteRoleAsync(id);
                
                if (roleDomain == null)
                {
                    _logger.LogWarning("Role with ID: {RoleId} not found for deletion", id);
                    throw new NotFoundException("Role", id);
                }

                var roleDto = new RolesDTO
                {
                    RoleID = roleDomain.RoleID,
                    RoleName = roleDomain.RoleName
                };

                _logger.LogInformation("Successfully deleted role with ID: {RoleId}", id);
                return Ok(roleDto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting role with ID: {RoleId}", id);
                throw;
            }
        }
    }
}
