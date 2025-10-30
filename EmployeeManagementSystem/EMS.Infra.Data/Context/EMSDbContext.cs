using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;

namespace EMS.Infra.Data.Context
{
    public class EMSDbContext: DbContext
    {
        public EMSDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}
