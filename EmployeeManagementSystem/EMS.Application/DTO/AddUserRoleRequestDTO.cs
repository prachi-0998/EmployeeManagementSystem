using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTO
{
    public class AddUserRoleRequestDTO
    {
        public int UserRoleID { get; set; }

        public int UserID { get; set; }

        public int RoleID { get; set; }

        public bool IsActive { get; set; }
    }
}
