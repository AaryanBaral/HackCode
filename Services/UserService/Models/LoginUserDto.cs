using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class LoginUserDto
    {
        [Required]
        public String Email { get; set; } = String.Empty;
        [Required]
        public String Password { get; set; } = string.Empty;
    }
}