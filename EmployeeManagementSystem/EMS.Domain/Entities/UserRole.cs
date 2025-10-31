using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Entities
{
       
       public class UserRole : BaseEntity
        {
            public int UserRoleID { get; set; }

            public int UserID { get; set; }

            public int RoleID { get; set; }

            public bool IsActive { get; set; }

          
            public Users? User { get; set; }   
            public Roles? Role { get; set; }   
        }
    


}
