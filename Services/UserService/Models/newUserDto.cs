using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class newUserDto
    {
        public String? UserName { get; set; }
        public String? Email { get; set; }
        public String? Token { get; set; }
    }
}