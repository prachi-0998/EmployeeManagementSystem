using EMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Repository
{
    public interface IRoleRepository
    {
        Task<List<Roles>> GetAllRolesAsync();
        Task<Roles?> GetRoleByIDAsync(int id);
        Task<Roles> CreateRoleAsync(Roles role);
        Task<Roles?> UpdateRoleAsync(int id, Roles role);
        Task<Roles?> DeleteRoleAsync(int id);
    }
}
