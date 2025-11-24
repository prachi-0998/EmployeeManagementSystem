using EMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Repository
{
    public interface IDepartmentRepository
    {
        Task<List<Departments>> GetAllDepartmentsAsync();
        Task<Departments?> GetDepartmentByIDAsync(int id);
        Task<Departments> CreateDepartmentAsync(Departments dept);
        Task<Departments?> UpdateDepartmentAsync(int id, Departments dept);
        Task<Departments?> DeleteDepartmentAsync(int id);
    }
}
