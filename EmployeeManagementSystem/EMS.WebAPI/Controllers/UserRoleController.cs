using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.Exceptions;
using EMS.Domain.Repository;
using EMS.Infra.Data;
using EMS.Infra.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserRoleController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        private readonly IUserRoleRepository urRepository;
        private readonly IMapper mapper;
        private readonly ILogger<UserRoleController> _logger;

        public UserRoleController(EMSDbContext dbContext, IUserRoleRepository urRepository, IMapper mapper, ILogger<UserRoleController> logger)
        {
            this.dbContext = dbContext;
            this.urRepository = urRepository;
            this.mapper = mapper;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserRoleDTO>>> GetAllUserRoleAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all user roles");
                var userRolesDomain = await urRepository.GetAllUserRoleAsync();
                var userRolesDto = mapper.Map<List<UserRoleDTO>>(userRolesDomain);
                _logger.LogInformation("Successfully retrieved {Count} user roles", userRolesDto.Count);
                return Ok(userRolesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all user roles");
                throw;
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserRoleDTO>> GetUserRoleByIDAsync([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Fetching user role with ID: {UserRoleId}", id);
                var userRole = await urRepository.GetUserRoleByIDAsync(id);

                if (userRole == null)
                {
                    _logger.LogWarning("User role with ID: {UserRoleId} not found", id);
                    throw new NotFoundException("UserRole", id);
                }

                var dto = new UserRoleDTO
                {
                    UserRoleID = userRole.UserRoleID,
                    UserID = userRole.UserID,
                    RoleID = userRole.RoleID,   
                    IsActive = userRole.IsActive
                };

                _logger.LogInformation("Successfully retrieved user role with ID: {UserRoleId}", id);
                return Ok(dto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user role with ID: {UserRoleId}", id);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<AddUserRoleRequestDTO>> CreateUserRoleAsync([FromBody] AddUserRoleRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for creating user role");
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Creating new user role for UserID: {UserId}, RoleID: {RoleId}", dto.UserID, dto.RoleID);
                var urDomain = new UserRole
                {
                    UserID = dto.UserID,
                    RoleID = dto.RoleID,
                    IsActive = dto.IsActive
                };
                
                urDomain = await urRepository.CreateUserRoleAsync(urDomain);

                var newurDto = new AddUserRoleRequestDTO
                {
                    UserID = urDomain.UserID,
                    RoleID = urDomain.RoleID,
                    IsActive = urDomain.IsActive
                };

                _logger.LogInformation("Successfully created user role with ID: {UserRoleId}", urDomain.UserRoleID);
                return CreatedAtAction(nameof(GetUserRoleByIDAsync), new { id = urDomain.UserRoleID }, newurDto);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user role");
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserRoleDTO>> UpdateUserRoleAsync([FromRoute] int id, [FromBody] AddUserRoleRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for updating user role with ID: {UserRoleId}", id);
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Updating user role with ID: {UserRoleId}", id);
                var urDomain = new UserRole
                {
                    UserID = dto.UserID,
                    RoleID = dto.RoleID,
                    IsActive = dto.IsActive
                };
                
                urDomain = await urRepository.UpdateUserRoleAsync(id, urDomain);

                if (urDomain == null)
                {
                    _logger.LogWarning("User role with ID: {UserRoleId} not found for update", id);
                    throw new NotFoundException("UserRole", id);
                }

                var urDto = new UserRoleDTO
                {
                    UserRoleID = urDomain.UserRoleID,
                    UserID = urDomain.UserID,
                    RoleID = urDomain.RoleID,
                    IsActive = urDomain.IsActive
                };

                _logger.LogInformation("Successfully updated user role with ID: {UserRoleId}", id);
                return Ok(urDto);
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
                _logger.LogError(ex, "Error occurred while updating user role with ID: {UserRoleId}", id);
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<UserRoleDTO>> DeleteUserRoleAsync([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Deleting user role with ID: {UserRoleId}", id);
                var urDomain = await urRepository.DeleteUserRoleAsync(id);

                if (urDomain == null)
                {
                    _logger.LogWarning("User role with ID: {UserRoleId} not found for deletion", id);
                    throw new NotFoundException("UserRole", id);
                }

                var urDto = new UserRoleDTO
                {
                    UserRoleID = urDomain.UserRoleID,
                    UserID = urDomain.UserID,
                    RoleID = urDomain.RoleID,
                    IsActive = urDomain.IsActive
                };

                _logger.LogInformation("Successfully deleted user role with ID: {UserRoleId}", id);
                return Ok(urDto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user role with ID: {UserRoleId}", id);
                throw;
            }
        }
    }
}
