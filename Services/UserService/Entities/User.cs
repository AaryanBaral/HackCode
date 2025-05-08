using System;
using Microsoft.AspNetCore.Identity;

namespace UserService.Entities
{
    public class User : IdentityUser<Guid>
    {
        

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
