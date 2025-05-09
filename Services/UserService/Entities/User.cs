
using System;
using Microsoft.AspNetCore.Identity;

namespace UserService.Entities
{
    public class User : IdentityUser<Guid>
    {
        public Guid Id {get; set;}
        public string UserName {get; set;}=string.Empty;
        public string Email {get; set;} =string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role {get; set;}
        public DateTime CreatedOn{get; set;} =DateTime.Now;
 
    }
}
