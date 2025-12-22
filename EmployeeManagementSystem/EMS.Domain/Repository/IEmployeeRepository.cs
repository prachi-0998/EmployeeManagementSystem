using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Domain.Repository
{
    public interface IEmployeeRepository
    {
        Task<List<Employees>> GetAllEmployeesAsync();
        Task<Employees?> GetEmployeeByIDAsync(int id);
        Task<Employees> CreateEmployeeAsync(Employees emp);
        Task<Employees?> UpdateEmployeeAsync(int id, Employees emp);
        Task<Employees?> DeleteEmployeeAsync(int id);
        
        /// <summary>
        /// Updates the user's email to a professional format when converted to an employee.
        /// Email format: firstname.lastname@company.com
        /// If duplicate names exist, appends incrementing numbers.
        /// </summary>
        /// <param name="userId">The user ID to update</param>
        /// <param name="firstName">Employee's first name</param>
        /// <param name="lastName">Employee's last name</param>
        /// <param name="companyDomain">Company domain (default: company.com)</param>
        /// <returns>The newly generated professional email address</returns>
        Task<string> UpdateUserEmailOnEmployeeConversionAsync(int userId, string firstName, string lastName, string companyDomain = "company.com");
    }
}
