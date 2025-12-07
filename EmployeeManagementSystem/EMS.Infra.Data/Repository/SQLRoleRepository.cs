using EMS.Domain.Entities;
using EMS.Domain.Repository;
using EMS.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Infra.Data.Repository
{
    public class SQLRoleRepository : IRoleRepository
    {
        private readonly EMSDbContext dbContext;
        //injecting DbContext class
        public SQLRoleRepository(EMSDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Roles>> GetAllRolesAsync()
        {
            return await dbContext.Roles.ToListAsync();
        }
        public async Task<Roles?> GetRoleByIDAsync(int id)
        {
            return await dbContext.Roles.FirstOrDefaultAsync(x => x.RoleID == id);
        }
        public async Task<Roles> CreateRoleAsync(Roles role)
        {
            await dbContext.Roles.AddAsync(role);
            await dbContext.SaveChangesAsync();
            return role;
        }

        public async Task<Roles?> UpdateRoleAsync(int id, Roles role)
        {
            var existingRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.RoleID == id);

            if (existingRole == null)
            {
                return null;
            }
            existingRole.RoleName = role.RoleName;
            existingRole.RoleID = role.RoleID;

            await dbContext.SaveChangesAsync();

            return existingRole;
        }

        public async Task<Roles?> DeleteRoleAsync(int id)
        {
            var existingRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.RoleID == id);

            if (existingRole == null)
            {
                return null;
            }

            dbContext.Roles.Remove(existingRole);
            await dbContext.SaveChangesAsync();

            return existingRole;
        }
    }
}
