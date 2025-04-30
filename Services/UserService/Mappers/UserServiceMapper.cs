using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserService.Entities;
using UserService.Models;

namespace UserService.Mappers
{
    public static class UserServiceMapper
    {
        public static User ToRegisterUser(this RegisterUserDto dto,PasswordHasher<User> Hasher){
            var user= new User{
                UserName=dto.UserName,
                Email=dto.Email,
            };
             user.PasswordHash=Hasher.HashPassword(user,dto.Password);
             return user;

        }

        public static User ToLoginUser(this LoginUserDto dto,PasswordHasher<User> Hasher){
            var user= new User{
                Email=dto.Email,
            };
            user.PasswordHash=Hasher.HashPassword(user,dto.Password);
            return user;
        }
    }
}