using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EMS.Domain.Entities;
using EMS.Domain.Repository;
using EMS.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace EMS.Infra.Data.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly EMSDbContext dbContext;

        public AuthRepository(EMSDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Users?> GetUserByUsernameAsync(string username)
        {
            return await dbContext.Users.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == username && u.IsActive);
        }

        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            return await dbContext.Users
                .FirstOrDefaultAsync(u => u.EmailID == email);
        }

        public async Task<Users> CreateUserAsync(Users user)
        {
            dbContext.Users.Add(user);
            return user;
        }

        public async Task<List<Roles>> GetRolesByNamesAsync(List<string> roleNames)
        {
            return await dbContext.Roles
                .Where(r => roleNames.Contains(r.RoleName))
                .ToListAsync();
        }

        public async Task AddUserRoleAsync(UserRole userRole)
        {
            dbContext.UserRole.Add(userRole);
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}