using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Entities
{
    public class Roles : BaseEntity
    {
        [Key]
        public int RoleID { get; set; }

        public string RoleName { get; set; }

        public bool IsActive { get; set; }

        public ICollection<UserRole>? UserRoles { get; set; }

        public ICollection<Employees>? Employees { get; set; }

    }
}
