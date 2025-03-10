using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechXpress.Application.Dto_s
{
    internal class RegisterUserDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsRegistered { get; set; }
    }
}
