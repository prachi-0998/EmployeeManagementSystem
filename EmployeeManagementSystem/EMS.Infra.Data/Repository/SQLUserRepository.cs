using EMS.Domain.Entities;
using EMS.Domain.Exceptions;
using EMS.Domain.Repository;
using EMS.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SQLUserRepository> _logger;

        public SQLUserRepository(EMSDbContext dbContext, ILogger<SQLUserRepository> logger)
        {
            this.dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogDebug("Retrieving all users from database");
                return await dbContext.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving all users");
                throw new DatabaseException("GetAllUsers", ex);
            }
        }

        public async Task<Users?> GetUserByIDAsync(int id)
        {
            try
            {
                _logger.LogDebug("Retrieving user with ID: {UserId}", id);
                return await dbContext.Users.FirstOrDefaultAsync(x => x.UserID == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving user with ID: {UserId}", id);
                throw new DatabaseException("GetUserByID", ex);
            }
        }

        public async Task<Users> CreateUserAsync(Users user)
        {
            try
            {
                _logger.LogDebug("Creating new user: {UserName}", user.UserName);
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully created user with ID: {UserId}", user.UserID);
                return user;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while creating user");
                throw new DatabaseException("CreateUser", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating user");
                throw new DatabaseException("CreateUser", ex);
            }
        }

        public async Task<Users?> UpdateUserAsync(int id, Users user)
        {
            try
            {
                _logger.LogDebug("Updating user with ID: {UserId}", id);
                var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserID == id);

                if (existingUser == null)
                {
                    _logger.LogDebug("User with ID: {UserId} not found for update", id);
                    return null;
                }

                existingUser.UserName = user.UserName;
                existingUser.EmailID = user.EmailID;
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully updated user with ID: {UserId}", id);
                return existingUser;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while updating user with ID: {UserId}", id);
                throw new DatabaseException("UpdateUser", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating user with ID: {UserId}", id);
                throw new DatabaseException("UpdateUser", ex);
            }
        }

        public async Task<Users?> DeleteAsync(int id)
        {
            try
            {
                _logger.LogDebug("Deleting user with ID: {UserId}", id);
                var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserID == id);

                if (existingUser == null)
                {
                    _logger.LogDebug("User with ID: {UserId} not found for deletion", id);
                    return null;
                }

                dbContext.Users.Remove(existingUser);
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully deleted user with ID: {UserId}", id);
                return existingUser;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while deleting user with ID: {UserId}", id);
                throw new DatabaseException("DeleteUser", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting user with ID: {UserId}", id);
                throw new DatabaseException("DeleteUser", ex);
            }
        }
    }
}
