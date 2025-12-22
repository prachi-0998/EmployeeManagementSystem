using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EMS.Domain.Entities;
using EMS.Domain.Exceptions;
using EMS.Domain.Repository;
using EMS.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EMS.Infra.Data.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly EMSDbContext dbContext;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(EMSDbContext dbContext, ILogger<AuthRepository> logger)
        {
            this.dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<Users?> GetUserByUsernameAsync(string username)
        {
            try
            {
                _logger.LogDebug("Retrieving user by username: {Username}", username);
                return await dbContext.Users.Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserName == username && u.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving user by username: {Username}", username);
                throw new DatabaseException("GetUserByUsername", ex);
            }
        }

        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            try
            {
                _logger.LogDebug("Retrieving user by email: {Email}", email);
                return await dbContext.Users
                    .FirstOrDefaultAsync(u => u.EmailID == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving user by email: {Email}", email);
                throw new DatabaseException("GetUserByEmail", ex);
            }
        }

        public async Task<Users> CreateUserAsync(Users user)
        {
            try
            {
                _logger.LogDebug("Creating new user: {Username}", user.UserName);
                dbContext.Users.Add(user);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding user to context: {Username}", user.UserName);
                throw new DatabaseException("CreateUser", ex);
            }
        }

        public async Task<List<Roles>> GetRolesByNamesAsync(List<string> roleNames)
        {
            try
            {
                _logger.LogDebug("Retrieving roles by names: {RoleNames}", string.Join(", ", roleNames));
                return await dbContext.Roles
                    .Where(r => roleNames.Contains(r.RoleName))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving roles by names");
                throw new DatabaseException("GetRolesByNames", ex);
            }
        }

        public async Task AddUserRoleAsync(UserRole userRole)
        {
            try
            {
                _logger.LogDebug("Adding user role for UserID: {UserId}, RoleID: {RoleId}", userRole.UserID, userRole.RoleID);
                dbContext.UserRole.Add(userRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding user role to context");
                throw new DatabaseException("AddUserRole", ex);
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                _logger.LogDebug("Saving changes to database");
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully saved changes to database");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while saving changes");
                throw new DatabaseException("SaveChanges", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving changes");
                throw new DatabaseException("SaveChanges", ex);
            }
        }
    }
}
