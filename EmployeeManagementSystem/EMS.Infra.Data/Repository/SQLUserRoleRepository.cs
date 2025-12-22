using EMS.Domain.Entities;
using EMS.Domain.Exceptions;
using EMS.Domain.Repository;
using EMS.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SQLUserRoleRepository> _logger;

        public SQLUserRoleRepository(EMSDbContext dbContext, ILogger<SQLUserRoleRepository> logger)
        {
            this.dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<List<UserRole>> GetAllUserRoleAsync()
        {
            try
            {
                _logger.LogDebug("Retrieving all user roles from database");
                return await dbContext.UserRole.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving all user roles");
                throw new DatabaseException("GetAllUserRoles", ex);
            }
        }

        public async Task<UserRole?> GetUserRoleByIDAsync(int id)
        {
            try
            {
                _logger.LogDebug("Retrieving user role with ID: {UserRoleId}", id);
                return await dbContext.UserRole.FirstOrDefaultAsync(x => x.UserRoleID == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving user role with ID: {UserRoleId}", id);
                throw new DatabaseException("GetUserRoleByID", ex);
            }
        }

        public async Task<UserRole> CreateUserRoleAsync(UserRole ur)
        {
            try
            {
                _logger.LogDebug("Creating new user role for UserID: {UserId}, RoleID: {RoleId}", ur.UserID, ur.RoleID);
                await dbContext.UserRole.AddAsync(ur);
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully created user role with ID: {UserRoleId}", ur.UserRoleID);
                return ur;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while creating user role");
                throw new DatabaseException("CreateUserRole", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating user role");
                throw new DatabaseException("CreateUserRole", ex);
            }
        }

        public async Task<UserRole?> UpdateUserRoleAsync(int id, UserRole ur)
        {
            try
            {
                _logger.LogDebug("Updating user role with ID: {UserRoleId}", id);
                var existingUR = await dbContext.UserRole.FirstOrDefaultAsync(x => x.UserRoleID == id);

                if (existingUR == null)
                {
                    _logger.LogDebug("User role with ID: {UserRoleId} not found for update", id);
                    return null;
                }

                existingUR.UserID = ur.UserID;
                existingUR.RoleID = ur.RoleID;
                existingUR.IsActive = ur.IsActive;
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully updated user role with ID: {UserRoleId}", id);
                return existingUR;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while updating user role with ID: {UserRoleId}", id);
                throw new DatabaseException("UpdateUserRole", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating user role with ID: {UserRoleId}", id);
                throw new DatabaseException("UpdateUserRole", ex);
            }
        }

        public async Task<UserRole?> DeleteUserRoleAsync(int id)
        {
            try
            {
                _logger.LogDebug("Deleting user role with ID: {UserRoleId}", id);
                var existingUR = await dbContext.UserRole.FirstOrDefaultAsync(x => x.UserRoleID == id);

                if (existingUR == null)
                {
                    _logger.LogDebug("User role with ID: {UserRoleId} not found for deletion", id);
                    return null;
                }

                dbContext.UserRole.Remove(existingUR);
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully deleted user role with ID: {UserRoleId}", id);
                return existingUR;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while deleting user role with ID: {UserRoleId}", id);
                throw new DatabaseException("DeleteUserRole", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting user role with ID: {UserRoleId}", id);
                throw new DatabaseException("DeleteUserRole", ex);
            }
        }
    }
}
