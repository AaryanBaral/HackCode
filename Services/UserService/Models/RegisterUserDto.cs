using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class RegisterUserDto
    {
        [Required]
        public String? UserName { get; set; }
        [Required]
        public String? Email { get; set; } 
        [Required]
        public String? Password { get; set; } 
    }
}