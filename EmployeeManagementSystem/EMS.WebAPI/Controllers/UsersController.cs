using AutoMapper;
using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.Exceptions;
using EMS.Domain.Repository;
using EMS.Infra.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;


namespace EMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly ILogger<UsersController> _logger;


        public UsersController(EMSDbContext dbContext, IUserRepository userRepository, IMapper mapper, ILogger<UsersController> logger)
        {
            this.dbContext = dbContext;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this._logger = logger;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        
        [HttpGet]
        [Authorize(Roles = "Admin, HR")]
        public async Task<ActionResult<UsersDTO>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                var usersDomain = await userRepository.GetAllUsersAsync();
                var usersDto = mapper.Map<List<UsersDTO>>(usersDomain);
                _logger.LogInformation("Successfully retrieved {Count} users", usersDto.Count);
                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all users");
                throw;
            }
        }

        /// <summary>
        /// Get a user by ID
        /// Admin/HR can view any user. Employee/Manager can only view their own profile.
        /// </summary>
        
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<UsersDTO>> GetUserByIdAsync([FromRoute] int id)
        {
            try
            {
                // Get current user's ID from JWT token
                var currentUserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out int currentUserId))
                {
                    _logger.LogWarning("Unable to extract user ID from token");
                    throw new UnauthorizedException("Invalid token");
                }

                // Check if user is Admin or HR
                var isAdminOrHR = User.IsInRole("Admin") || User.IsInRole("HR");

               
                if (!isAdminOrHR && currentUserId != id)
                {
                    _logger.LogWarning("User {CurrentUserId} attempted to access user {RequestedUserId} without permission", 
                        currentUserId, id);
                    throw new UnauthorizedException("You can only view your own profile");
                }

                _logger.LogInformation("Fetching user with ID: {UserId}", id);
                var userDomain = await userRepository.GetUserByIDAsync(id);

                if (userDomain == null)
                {
                    _logger.LogWarning("User with ID: {UserId} not found", id);
                    throw new NotFoundException("User", id);
                }

                var userDto = mapper.Map<UsersDTO>(userDomain);
                _logger.LogInformation("Successfully retrieved user with ID: {UserId}", id);
                return Ok(userDto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (UnauthorizedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID: {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new user 
        /// </summary>
        
        [HttpPost]
        [Authorize(Roles = "Admin, HR")]
        public async Task<ActionResult<UsersDTO>> CreateUserAsync([FromBody] AddUserRequestDTO userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for creating user");
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Creating new user: {UserName}", userDto.UserName);
                var userDomain = mapper.Map<Users>(userDto);
                userDomain = await userRepository.CreateUserAsync(userDomain);

                var newuserDto = mapper.Map<UsersDTO>(userDomain);
                _logger.LogInformation("Successfully created user with ID: {UserId}", newuserDto.UserID);
                return Created($"/api/Users/{newuserDto.UserID}", newuserDto);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                throw;
            }
        }

        /// <summary>
        /// Update a user
        /// </summary>
        
        
        [HttpPut]
        [Authorize(Roles = "Admin, HR")]
        [Route("{id}")] 
        public async Task<ActionResult<UsersDTO>> UpdateUserAsync([FromRoute] int id, [FromBody] UpdateUserRequestDTO updateUserRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for updating user with ID: {UserId}", id);
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Updating user with ID: {UserId}", id);
                var userDomain = mapper.Map<Users>(updateUserRequestDto);
                userDomain = await userRepository.UpdateUserAsync(id, userDomain);
               
                if (userDomain == null)
                {    
                    _logger.LogWarning("User with ID: {UserId} not found for update", id);
                    throw new NotFoundException("User", id);
                }

                var userDto = mapper.Map<UsersDTO>(userDomain);
                _logger.LogInformation("Successfully updated user with ID: {UserId}", id);
                return Ok(userDto);
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
                _logger.LogError(ex, "Error occurred while updating user with ID: {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        
        [HttpDelete]
        [Authorize(Roles = "Admin, HR")]
        [Route("{id}")]
        public async Task<ActionResult<UsersDTO>> DeleteUserAsync([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("Deleting user with ID: {UserId}", id);
                var userDomain = await userRepository.DeleteAsync(id);

                if (userDomain == null)
                {
                    _logger.LogWarning("User with ID: {UserId} not found for deletion", id);
                    throw new NotFoundException("User", id);
                }

                var userDto = mapper.Map<UsersDTO>(userDomain);
                _logger.LogInformation("Successfully deleted user with ID: {UserId}", id);
                return Ok(userDto);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);
                throw;
            }
        }
    }
}
