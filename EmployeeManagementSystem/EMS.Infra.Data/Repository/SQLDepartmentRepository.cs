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
    public class SQLDepartmentRepository : IDepartmentRepository
    {
        private readonly EMSDbContext dbContext;
        //injecting DbContext class
        public SQLDepartmentRepository(EMSDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Departments>> GetAllDepartmentsAsync()
        {
            return await dbContext.Departments.ToListAsync();
        }
        public async Task<Departments?> GetDepartmentByIDAsync(int id)
        {
            return await dbContext.Departments.FirstOrDefaultAsync(x => x.DepartmentID == id);
        }
        public async Task<Departments> CreateDepartmentAsync(Departments dept)
        {
            await dbContext.Departments.AddAsync(dept);
            await dbContext.SaveChangesAsync();
            return dept;
        }

        public async Task<Departments?> UpdateDepartmentAsync(int id, Departments dept)
        {
            var existingDept = await dbContext.Departments.FirstOrDefaultAsync(x => x.DepartmentID == id);

            if (existingDept == null)
            {
                return null;
            }
            existingDept.DepartmentName = dept.DepartmentName;
            
            await dbContext.SaveChangesAsync();

            return existingDept;
        }

        public async Task<Departments?> DeleteDepartmentAsync(int id)
        {
            var existingDept = await dbContext.Departments.FirstOrDefaultAsync(x => x.DepartmentID == id);

            if (existingDept == null)
            {
                return null;
            }

            dbContext.Departments.Remove(existingDept);
            await dbContext.SaveChangesAsync();

            return existingDept;
        }

        
    }
}

