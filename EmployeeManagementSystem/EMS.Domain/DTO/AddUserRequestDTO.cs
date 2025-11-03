using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.DTO
{
    public class AddUserRequestDTO
    {
        public int UserID { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string EmailID { get; set; } = string.Empty;
    }
}
