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
    public class SQLDepartmentRepository : IDepartmentRepository
    {
        private readonly EMSDbContext dbContext;
        private readonly ILogger<SQLDepartmentRepository> _logger;

        public SQLDepartmentRepository(EMSDbContext dbContext, ILogger<SQLDepartmentRepository> logger)
        {
            this.dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<List<Departments>> GetAllDepartmentsAsync()
        {
            try
            {
                _logger.LogDebug("Retrieving all departments from database");
                return await dbContext.Departments.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving all departments");
                throw new DatabaseException("GetAllDepartments", ex);
            }
        }

        public async Task<Departments?> GetDepartmentByIDAsync(int id)
        {
            try
            {
                _logger.LogDebug("Retrieving department with ID: {DepartmentId}", id);
                return await dbContext.Departments.FirstOrDefaultAsync(x => x.DepartmentID == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving department with ID: {DepartmentId}", id);
                throw new DatabaseException("GetDepartmentByID", ex);
            }
        }

        public async Task<Departments> CreateDepartmentAsync(Departments dept)
        {
            try
            {
                _logger.LogDebug("Creating new department: {DepartmentName}", dept.DepartmentName);
                await dbContext.Departments.AddAsync(dept);
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully created department with ID: {DepartmentId}", dept.DepartmentID);
                return dept;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while creating department");
                throw new DatabaseException("CreateDepartment", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating department");
                throw new DatabaseException("CreateDepartment", ex);
            }
        }

        public async Task<Departments?> UpdateDepartmentAsync(int id, Departments dept)
        {
            try
            {
                _logger.LogDebug("Updating department with ID: {DepartmentId}", id);
                var existingDept = await dbContext.Departments.FirstOrDefaultAsync(x => x.DepartmentID == id);

                if (existingDept == null)
                {
                    _logger.LogDebug("Department with ID: {DepartmentId} not found for update", id);
                    return null;
                }

                existingDept.DepartmentName = dept.DepartmentName;
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully updated department with ID: {DepartmentId}", id);
                return existingDept;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while updating department with ID: {DepartmentId}", id);
                throw new DatabaseException("UpdateDepartment", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating department with ID: {DepartmentId}", id);
                throw new DatabaseException("UpdateDepartment", ex);
            }
        }

        public async Task<Departments?> DeleteDepartmentAsync(int id)
        {
            try
            {
                _logger.LogDebug("Deleting department with ID: {DepartmentId}", id);
                var existingDept = await dbContext.Departments.FirstOrDefaultAsync(x => x.DepartmentID == id);

                if (existingDept == null)
                {
                    _logger.LogDebug("Department with ID: {DepartmentId} not found for deletion", id);
                    return null;
                }

                dbContext.Departments.Remove(existingDept);
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully deleted department with ID: {DepartmentId}", id);
                return existingDept;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while deleting department with ID: {DepartmentId}", id);
                throw new DatabaseException("DeleteDepartment", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting department with ID: {DepartmentId}", id);
                throw new DatabaseException("DeleteDepartment", ex);
            }
        }
    }
}
