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
    public class SQLUserRepository : IUserRepository
    {
        private readonly EMSDbContext dbContext;
        //injecting DbContext class
        public SQLUserRepository(EMSDbContext dbContext)
        {
         this.dbContext = dbContext;    
        }
        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await dbContext.Users.ToListAsync();
        }
        public async Task<Users?> GetUserByIDAsync(int id)
        {
            return await dbContext.Users.FirstOrDefaultAsync(x => x.UserID == id);
        }
        public async Task<Users> CreateUserAsync(Users user)
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<Users?> UpdateUserAsync(int id, Users user)
        {
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserID == id);

            if (existingUser == null)
            {
                return null;
            }
            existingUser.UserName = user.UserName;
            existingUser.EmailID = user.EmailID;

            await dbContext.SaveChangesAsync();

            return existingUser;
        }

        public async Task<Users?> DeleteAsync(int id)
        {
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserID == id);

            if (existingUser == null)
            {
                return null;
            }

            dbContext.Users.Remove(existingUser);
            await dbContext.SaveChangesAsync();

            return existingUser;
        }
    }
}
