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

        //GET: Get all users
        [HttpGet]
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

        //GET: Get user by id
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<UsersDTO>> GetUserByIdAsync([FromRoute] int id)
        {
            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID: {UserId}", id);
                throw;
            }
        }

        //POST: To create a new user
        [HttpPost]
        public async Task<ActionResult<AddUserRequestDTO>> CreateUserAsync([FromBody] AddUserRequestDTO userDto)
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

        //PUT: Update the resource
        [HttpPut]
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

        //DELETE: Delete a user
        [HttpDelete]
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
