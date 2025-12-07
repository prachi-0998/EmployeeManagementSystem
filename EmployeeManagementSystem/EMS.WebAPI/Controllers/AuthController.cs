using EMS.Application.DTO;
using EMS.Application.Services;
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

        public AuthController(EMSDbContext dbContext, IAuthService authService)
        {
            this.dbContext = dbContext;
            this.authService = authService;
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
                    return BadRequest(ModelState);
                }

                var user = await authService.RegisterAsync(registerRequest);

                return Ok(new
                {
                    Message = "User registered successfully",
                    User = user
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Registration failed: {ex.Message}" });
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
                    return BadRequest(ModelState);
                }

                var result = await authService.LoginAsync(loginRequest);

                if (result == null)
                {
                    return Unauthorized(new { Message = "Invalid username or password" });
                }

                return Ok(new
                {
                    Message = "Login successful",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Login failed: {ex.Message}" });
            }
        }

        /// <summary>
        /// Test endpoint to verify authentication is working
        /// </summary>
        /// <returns>Success message</returns>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { Message = "Auth controller is working!", Timestamp = DateTime.UtcNow });
        }
    }
}