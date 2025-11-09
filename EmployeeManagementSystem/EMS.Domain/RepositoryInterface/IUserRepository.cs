using EMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.RepositoryInterface
{
    public interface IUserRepository
    {
        Task<List<Users>> GetAllUsersAsync();

       // Task<Users> GetUserByIdAsync(int id);
    }
}
