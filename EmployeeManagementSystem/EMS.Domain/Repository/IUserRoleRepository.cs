using EMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Repository
{
    public interface IUserRoleRepository
    {
        Task<List<UserRole>> GetAllUserRoleAsync();
        Task<UserRole?> GetUserRoleByIDAsync(int id);
        Task<UserRole> CreateUserRoleAsync(UserRole dept);
        Task<UserRole?> UpdateUserRoleAsync(int id, UserRole dept);
        Task<UserRole?> DeleteUserRoleAsync(int id);
       
    }
}
