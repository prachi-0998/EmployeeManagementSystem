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
    public class SQLRoleRepository : IRoleRepository
    {
        private readonly EMSDbContext dbContext;
        private readonly ILogger<SQLRoleRepository> _logger;

        public SQLRoleRepository(EMSDbContext dbContext, ILogger<SQLRoleRepository> logger)
        {
            this.dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<List<Roles>> GetAllRolesAsync()
        {
            try
            {
                _logger.LogDebug("Retrieving all roles from database");
                return await dbContext.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving all roles");
                throw new DatabaseException("GetAllRoles", ex);
            }
        }

        public async Task<Roles?> GetRoleByIDAsync(int id)
        {
            try
            {
                _logger.LogDebug("Retrieving role with ID: {RoleId}", id);
                return await dbContext.Roles.FirstOrDefaultAsync(x => x.RoleID == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving role with ID: {RoleId}", id);
                throw new DatabaseException("GetRoleByID", ex);
            }
        }

        public async Task<Roles> CreateRoleAsync(Roles role)
        {
            try
            {
                _logger.LogDebug("Creating new role: {RoleName}", role.RoleName);
                await dbContext.Roles.AddAsync(role);
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully created role with ID: {RoleId}", role.RoleID);
                return role;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while creating role");
                throw new DatabaseException("CreateRole", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating role");
                throw new DatabaseException("CreateRole", ex);
            }
        }

        public async Task<Roles?> UpdateRoleAsync(int id, Roles role)
        {
            try
            {
                _logger.LogDebug("Updating role with ID: {RoleId}", id);
                var existingRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.RoleID == id);

                if (existingRole == null)
                {
                    _logger.LogDebug("Role with ID: {RoleId} not found for update", id);
                    return null;
                }

                existingRole.RoleName = role.RoleName;
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully updated role with ID: {RoleId}", id);
                return existingRole;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while updating role with ID: {RoleId}", id);
                throw new DatabaseException("UpdateRole", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating role with ID: {RoleId}", id);
                throw new DatabaseException("UpdateRole", ex);
            }
        }

        public async Task<Roles?> DeleteRoleAsync(int id)
        {
            try
            {
                _logger.LogDebug("Deleting role with ID: {RoleId}", id);
                var existingRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.RoleID == id);

                if (existingRole == null)
                {
                    _logger.LogDebug("Role with ID: {RoleId} not found for deletion", id);
                    return null;
                }

                dbContext.Roles.Remove(existingRole);
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully deleted role with ID: {RoleId}", id);
                return existingRole;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while deleting role with ID: {RoleId}", id);
                throw new DatabaseException("DeleteRole", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting role with ID: {RoleId}", id);
                throw new DatabaseException("DeleteRole", ex);
            }
        }
    }
}
