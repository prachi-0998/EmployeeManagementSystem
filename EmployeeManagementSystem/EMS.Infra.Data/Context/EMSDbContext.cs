using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;

namespace EMS.Infra.Data.Context
{
    public class EMSDbContext: DbContext
    {
        // we will see the use of this ctor later when we create new connection string will inject the connection through program.cs file
        public EMSDbContext(DbContextOptions<EMSDbContext> options) : base(options)
        {
            
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<UserRole> UserRole { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seed data for departments
            var depts = new List<Departments>();
            {
                new Departments()
                {
                    DepartmentID = 1,
                    DepartmentName = "HR"
                };

                new Departments()
                {
                    DepartmentID = 2,
                    DepartmentName = "IT"
                };

                new Departments()
                {
                    DepartmentID = 3,
                    DepartmentName = "Finance"
                };

                new Departments()
                {
                    DepartmentID = 4,
                    DepartmentName = "Technology"
                };
            };


            modelBuilder.Entity<Departments>().HasData(depts);
        }

    }
}
