using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;

namespace EMS.Infra.Data.Context
{
    public class EMSDbContext: DbContext
    {
        // we will see the use of this ctor later when we create new connection string will inject the connection through program.cs file
        public EMSDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<UserRole> UserRole { get; set; }

    }
}
