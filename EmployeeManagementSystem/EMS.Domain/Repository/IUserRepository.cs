using EMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Repository
{
    public interface IUserRepository
    {
        Task<List<Users>> GetAllUsersAsync();
        Task<Users?> GetUserByIDAsync(int id);
        Task<Users> CreateUserAsync(Users user);
        Task<Users?> UpdateUserAsync(int id, Users user);
        Task<Users?> DeleteAsync(int id);
    }
}
