using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTO
{
    public class AddUserRequestDTO
    {
        public string UserName { get; set; } = string.Empty;

        public string EmailID { get; set; } = string.Empty;
    }
}
