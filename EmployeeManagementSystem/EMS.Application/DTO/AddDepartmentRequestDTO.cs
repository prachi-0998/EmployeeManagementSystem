using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTO
{
    public class AddDepartmentRequestDTO
    {
        public int DepartmentID { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
