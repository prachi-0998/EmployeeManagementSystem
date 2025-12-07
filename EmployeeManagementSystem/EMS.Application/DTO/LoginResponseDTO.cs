using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTO
{
    public class LoginResponseDTO
    {
        public string JwtToken { get; set; } = string.Empty;
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string EmailID { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
