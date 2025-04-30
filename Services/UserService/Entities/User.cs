using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Entities
{
    public class User
    {
        public Guid Id {get; set;}
        public  String UserName{get; set;}=String.Empty;
        public String Email {get; set;} =String.Empty;
        public String PasswordHash { get; set; } = string.Empty;
        public UserRole Role {get; set;}
        public DateTime CreatedOn{get; set;} =DateTime.Now;
    }
     public enum UserRole
    {
        Admin,
        Moderator,
        User,
        Guest
    }
}