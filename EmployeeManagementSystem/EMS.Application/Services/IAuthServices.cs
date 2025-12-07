using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Application.DTO;

namespace EMS.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequest);
        Task<UsersDTO> RegisterAsync(RegisterRequestDTO registerRequest);
        string GenerateJwtToken(int userId, string userName, List<string> roles);

    }
}
