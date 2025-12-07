using EMS.Domain.Entities;
using EMS.Domain.Repository;
using EMS.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Infra.Data.Repository
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly EMSDbContext dbContext;
        //injecting DbContext class
        public SQLEmployeeRepository(EMSDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Employees>> GetAllEmployeesAsync()
        {
            return await dbContext.Employees.ToListAsync();
        }
        public async Task<Employees?> GetEmployeeByIDAsync(int id)
        {
            return await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);
        }
        public async Task<Employees> CreateEmployeeAsync(Employees emp)
        {
            await dbContext.Employees.AddAsync(emp);
            await dbContext.SaveChangesAsync();
            return emp;
        }

        public async Task<Employees?> UpdateEmployeeAsync(int id, Employees emp)
        {
            var existingEmp = await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);

            if (existingEmp == null)
            {
                return null;
            }
            existingEmp.FirstName = emp.FirstName;
            existingEmp.LastName = emp.LastName;
            existingEmp.DepartmentID = emp.DepartmentID;
            existingEmp.RoleID = emp.RoleID;
            existingEmp.IsActive = emp.IsActive;

            await dbContext.SaveChangesAsync();

            return existingEmp;
        }

        public async Task<Employees?> DeleteEmployeeAsync(int id)
        {
            var existingEmp = await dbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);

            if (existingEmp == null)
            {
                return null;
            }

            dbContext.Employees.Remove(existingEmp);
            await dbContext.SaveChangesAsync();

            return existingEmp;
        }

    }
}
