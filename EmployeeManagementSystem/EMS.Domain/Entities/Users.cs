using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Entities
{
        public class Users : BaseEntity
        {
            public int UserID { get; set; }

            public string UserName { get; set; } = string.Empty;

            public string EmailID { get; set; } = string.Empty;

            public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

            public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

            public bool IsActive { get; set; }

            public ICollection<UserRole>? UserRoles { get; set; } 
        }
    

}
