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
    }
}
