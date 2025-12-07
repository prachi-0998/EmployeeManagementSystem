using EMS.Application.DTO;
using EMS.Domain.Entities;
using EMS.Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository authRepository;
        private readonly IConfiguration configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            this.authRepository = authRepository;
            this.configuration = configuration;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
        {
            var user = await authRepository.GetUserByUsernameAsync(loginRequest.UserName);

            if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            var roles = user.UserRoles?.Where(ur => ur.IsActive)
                .Select(ur => ur.Role?.RoleName ?? string.Empty)
                .Where(r => !string.IsNullOrEmpty(r))
                .ToList() ?? new List<string>();

            var token = GenerateJwtToken(user.UserID, user.UserName, roles);

            return new LoginResponseDTO
            {
                JwtToken = token,
                UserID = user.UserID,
                UserName = user.UserName,
                EmailID = user.EmailID,
                Roles = roles
            };
        }

        public async Task<UsersDTO> RegisterAsync(RegisterRequestDTO registerRequest)
        {
            // Check if user already exists
            var existingUserByUsername = await authRepository.GetUserByUsernameAsync(registerRequest.UserName);
            var existingUserByEmail = await authRepository.GetUserByEmailAsync(registerRequest.EmailID);

            if (existingUserByUsername != null || existingUserByEmail != null)
            {
                throw new InvalidOperationException("User with this username or email already exists");
            }

            CreatePasswordHash(registerRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new Users
            {
                UserName = registerRequest.UserName,
                EmailID = registerRequest.EmailID,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsActive = true
            };

            await authRepository.CreateUserAsync(user);
            await authRepository.SaveChangesAsync();

            // Assign roles if provided
            if (registerRequest.Roles?.Any() == true)
            {
                var roleEntities = await authRepository.GetRolesByNamesAsync(registerRequest.Roles);

                foreach (var role in roleEntities)
                {
                    var userRole = new UserRole
                    {
                        UserID = user.UserID,
                        RoleID = role.RoleID,
                        IsActive = true
                    };
                    await authRepository.AddUserRoleAsync(userRole);
                }
                await authRepository.SaveChangesAsync();
            }

            return new UsersDTO
            {
                UserID = user.UserID,
                UserName = user.UserName,
                EmailID = user.EmailID
            };
        }

        public string GenerateJwtToken(int userId, string userName, List<string> roles)
        {
            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtAudience = configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new ArgumentNullException("JWT configuration is missing");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}