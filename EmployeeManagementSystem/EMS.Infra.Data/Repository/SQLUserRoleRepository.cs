using EMS.Domain.Entities;
using EMS.Domain.Repository;
using EMS.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Infra.Data.Repository
{
    public class SQLUserRoleRepository : IUserRoleRepository
    {
        private readonly EMSDbContext dbContext;
        //injecting DbContext class
        public SQLUserRoleRepository(EMSDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<UserRole>> GetAllUserRoleAsync()
        {
            return await dbContext.UserRole.ToListAsync();
        }
        public async Task<UserRole?> GetUserRoleByIDAsync(int id)
        {
            return await dbContext.UserRole.FirstOrDefaultAsync(x => x.UserRoleID == id);
        }
        public async Task<UserRole> CreateUserRoleAsync(UserRole ur)
        {
            await dbContext.UserRole.AddAsync(ur);
            await dbContext.SaveChangesAsync();
            return ur;
        }

        public async Task<UserRole?> UpdateUserRoleAsync(int id, UserRole ur)
        {
            var existingUR = await dbContext.UserRole.FirstOrDefaultAsync(x => x.UserRoleID == id);

            if (existingUR == null)
            {
                return null;
            }
            existingUR.UserID = ur.UserID;
            existingUR.RoleID = ur.RoleID;

            await dbContext.SaveChangesAsync();

            return existingUR;
        }

        public async Task<UserRole?> DeleteUserRoleAsync(int id)
        {
            var existingUR = await dbContext.UserRole.FirstOrDefaultAsync(x => x.UserRoleID == id);

            if (existingUR == null)
            {
                return null;
            }

            dbContext.UserRole.Remove(existingUR);
            await dbContext.SaveChangesAsync();

            return existingUR;
        }
    }
}
