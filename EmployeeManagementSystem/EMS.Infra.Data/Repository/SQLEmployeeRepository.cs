using EMS.Domain.Entities;
using EMS.Domain.Exceptions;
using EMS.Domain.Repository;
using EMS.Infra.Data.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Infra.Data.Repository
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly EMSDbContext dbContext;
        private readonly ILogger<SQLEmployeeRepository> _logger;

        public SQLEmployeeRepository(EMSDbContext dbContext, ILogger<SQLEmployeeRepository> logger)
        {
            this.dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<List<Employees>> GetAllEmployeesAsync()
        {
            try
            {
                _logger.LogDebug("Retrieving all employees from database");
                return await dbContext.Employees.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving all employees");
                throw new DatabaseException("GetAllEmployees", ex);
            }
        }

        public async Task<Employees?> GetEmployeeByIDAsync(int id)
        {
            try
            {
                _logger.LogDebug("Retrieving employee with ID: {EmployeeId}", id);
                return await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while retrieving employee with ID: {EmployeeId}", id);
                throw new DatabaseException("GetEmployeeByID", ex);
            }
        }

        public async Task<Employees> CreateEmployeeAsync(Employees emp)
        {
            try
            {
                _logger.LogDebug("Creating new employee: {FirstName} {LastName}", emp.FirstName, emp.LastName);
                await dbContext.Employees.AddAsync(emp);
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully created employee with ID: {EmployeeId}", emp.EmployeeID);

                // Update the user's email to professional email when converted to employee
                if (emp.UserID > 0)
                {
                    _logger.LogDebug("Converting user {UserId} email to professional email", emp.UserID);
                    await UpdateUserEmailOnEmployeeConversionAsync(emp.UserID, emp.FirstName, emp.LastName);
                }

                return emp;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while creating employee");
                throw new DatabaseException("CreateEmployee", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating employee");
                throw new DatabaseException("CreateEmployee", ex);
            }
        }

        public async Task<Employees?> UpdateEmployeeAsync(int id, Employees emp)
        {
            try
            {
                _logger.LogDebug("Updating employee with ID: {EmployeeId}", id);
                var existingEmp = await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);

                if (existingEmp == null)
                {
                    _logger.LogDebug("Employee with ID: {EmployeeId} not found for update", id);
                    return null;
                }

                existingEmp.FirstName = emp.FirstName;
                existingEmp.LastName = emp.LastName;
                existingEmp.DepartmentID = emp.DepartmentID;
                existingEmp.RoleID = emp.RoleID;
                existingEmp.IsActive = emp.IsActive;

                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully updated employee with ID: {EmployeeId}", id);
                return existingEmp;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while updating employee with ID: {EmployeeId}", id);
                throw new DatabaseException("UpdateEmployee", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating employee with ID: {EmployeeId}", id);
                throw new DatabaseException("UpdateEmployee", ex);
            }
        }

        public async Task<Employees?> DeleteEmployeeAsync(int id)
        {
            try
            {
                _logger.LogDebug("Deleting employee with ID: {EmployeeId}", id);
                var existingEmp = await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);

                if (existingEmp == null)
                {
                    _logger.LogDebug("Employee with ID: {EmployeeId} not found for deletion", id);
                    return null;
                }

                dbContext.Employees.Remove(existingEmp);
                await dbContext.SaveChangesAsync();
                _logger.LogDebug("Successfully deleted employee with ID: {EmployeeId}", id);
                return existingEmp;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while deleting employee with ID: {EmployeeId}", id);
                throw new DatabaseException("DeleteEmployee", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting employee with ID: {EmployeeId}", id);
                throw new DatabaseException("DeleteEmployee", ex);
            }
        }

        public async Task<string> UpdateUserEmailOnEmployeeConversionAsync(int userId, string firstName, string lastName, string companyDomain = "company.com")
        {
            try
            {
                _logger.LogDebug("Updating email for user {UserId} to professional format: {FirstName}.{LastName}@{Domain}", 
                    userId, firstName, lastName, companyDomain);

                var userIdParam = new SqlParameter("@UserID", SqlDbType.Int) { Value = userId };
                var firstNameParam = new SqlParameter("@FirstName", SqlDbType.NVarChar, 100) { Value = firstName };
                var lastNameParam = new SqlParameter("@LastName", SqlDbType.NVarChar, 100) { Value = lastName };
                var domainParam = new SqlParameter("@CompanyDomain", SqlDbType.NVarChar, 100) { Value = companyDomain };
                var newEmailParam = new SqlParameter("@NewEmail", SqlDbType.NVarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };

                await dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[SP_UpdateUserEmailOnEmployeeConversion] @UserID, @FirstName, @LastName, @CompanyDomain, @NewEmail OUTPUT",
                    userIdParam, firstNameParam, lastNameParam, domainParam, newEmailParam);

                var newEmail = newEmailParam.Value?.ToString() ?? string.Empty;
                _logger.LogInformation("Successfully updated email for user {UserId} to {NewEmail}", userId, newEmail);

                return newEmail;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while updating email for user {UserId}", userId);
                throw new DatabaseException("UpdateUserEmailOnEmployeeConversion", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating email for user {UserId}", userId);
                throw new DatabaseException("UpdateUserEmailOnEmployeeConversion", ex);
            }
        }
    }
}
