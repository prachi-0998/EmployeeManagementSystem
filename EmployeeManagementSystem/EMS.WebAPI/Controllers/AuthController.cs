using EMS.Application.DTO;
using EMS.Application.Services;
using EMS.Domain.Exceptions;
using EMS.Infra.Data.Context;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EMSDbContext dbContext;
        private readonly IAuthService authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(EMSDbContext dbContext, IAuthService authService, ILogger<AuthController> logger)
        {
            this.dbContext = dbContext;
            this.authService = authService;
            this._logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registerRequest">User registration details</param>
        /// <returns>Created user information</returns>
        [HttpPost("register")]
        public async Task<ActionResult<UsersDTO>> RegisterAsync([FromBody] RegisterRequestDTO registerRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for user registration");
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Attempting to register new user: {UserName}", registerRequest.UserName);
                var user = await authService.RegisterAsync(registerRequest);

                _logger.LogInformation("Successfully registered user: {UserName} with ID: {UserId}", user.UserName, user.UserID);
                return Ok(new
                {
                    Message = "User registered successfully",
                    User = user
                });
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate user registration attempt: {UserName}", registerRequest.UserName);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation during registration: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration: {UserName}", registerRequest.UserName);
                throw;
            }
        }

        /// <summary>
        /// Login user and get JWT token
        /// </summary>
        /// <param name="loginRequest">User login credentials</param>
        /// <returns>JWT token and user information</returns>
        [HttpPost("login")]
        public async Task<ActionResult<LoginRequestDTO>> LoginAsync([FromBody] LoginRequestDTO loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for user login");
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    throw new ValidationException(errors);
                }

                _logger.LogInformation("Login attempt for user: {UserName}", loginRequest.UserName);
                var result = await authService.LoginAsync(loginRequest);

                if (result == null)
                {
                    _logger.LogWarning("Failed login attempt for user: {UserName}", loginRequest.UserName);
                    throw new UnauthorizedException("Invalid username or password");
                }

                _logger.LogInformation("Successful login for user: {UserName}", loginRequest.UserName);
                return Ok(new
                {
                    Message = "Login successful",
                    Data = result
                });
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (UnauthorizedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for user: {UserName}", loginRequest.UserName);
                throw;
            }
        }

        /// <summary>
        /// Test endpoint to verify authentication is working
        /// </summary>
        /// <returns>Success message</returns>
        [HttpGet("test")]
        public IActionResult Test()
        {
            try
            {
                _logger.LogInformation("Auth test endpoint called");
                return Ok(new { Message = "Auth controller is working!", Timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in auth test endpoint");
                throw;
            }
        }
    }
}
