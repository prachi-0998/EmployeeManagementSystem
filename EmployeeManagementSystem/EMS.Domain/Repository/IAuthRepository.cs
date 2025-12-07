using EMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Repository
{
    public interface IAuthRepository
    {
        Task<Users?> GetUserByUsernameAsync(string username);
        Task<Users?> GetUserByEmailAsync(string email);
        Task<Users> CreateUserAsync(Users user);
        Task<List<Roles>> GetRolesByNamesAsync(List<string> roleNames);
        Task AddUserRoleAsync(UserRole userRole);
        Task SaveChangesAsync();
    }
}
