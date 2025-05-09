<<<<<<< HEAD
=======
using System;
using Microsoft.AspNetCore.Identity;

>>>>>>> 4bbba4ced183b0d97c5de803738911a17dbd1df8
namespace UserService.Entities
{
    public class User : IdentityUser<Guid>
    {
<<<<<<< HEAD
        public Guid Id {get; set;}
        public string UserName {get; set;}=string.Empty;
        public string Email {get; set;} =string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role {get; set;}
        public DateTime CreatedOn{get; set;} =DateTime.Now;
=======
        

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
>>>>>>> 4bbba4ced183b0d97c5de803738911a17dbd1df8
    }
}
