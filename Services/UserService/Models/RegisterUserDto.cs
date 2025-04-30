using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class RegisterUserDto
    {
        public  String UserName{get; set;}=String.Empty;
        public String Email{get; set;}=String.Empty;
        public String Password { get; set; } = string.Empty;
    }
}