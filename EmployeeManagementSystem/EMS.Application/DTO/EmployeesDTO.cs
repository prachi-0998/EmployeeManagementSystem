using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTO
{
    public class EmployeesDTO
    {
        public int EmployeeID { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string ContactNo { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public int DepartmentID { get; set; }

        public int RoleID { get; set; }

        public DateTime HireDate { get; set; }

        public decimal Salary { get; set; }

        public int? ManagerID { get; set; }

        public bool IsActive { get; set; }

        public int UserID { get; set; }

    }
}
