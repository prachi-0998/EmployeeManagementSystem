using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTO
{
    public class RegisterRequestDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string EmailID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
