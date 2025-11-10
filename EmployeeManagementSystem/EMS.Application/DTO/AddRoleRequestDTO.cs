using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTO
{
    public class AddRoleRequestDTO
    {
        public int RoleID { get; set; }

        public string RoleName { get; set; }

        public bool IsActive { get; set; }
    }
}
