using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using UserService.Entities;
using UserService.Model;

namespace UserService.Mappers
{
    public class UserMapper
    {
        public static User ToRegisterUser(RegisterDto dto)
        {
            return new User
            {
                Email = dto.Email,
                UserName = dto.UserName,
                CreatedAt = DateTime.UtcNow,
                
            };
        }
        public static User ToLoginUser(LoginDto dto)
        {
            return new User
            {
                Email = dto.Email,
                PasswordHash = dto.Password
            };
        }
        
    }
}