using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserService.Data;
using UserService.Entities;
using UserService.Interfaces;
using UserService.Mappers;
using UserService.Models;
namespace UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
    
        public UserRepository(UserDbContext context)
        {
            _context = context;
        
        }
//Register the user using email,username and password
        public async Task<User?> RegisterUser(User request)
        {
            await _context.AddAsync(request);
            await _context.SaveChangesAsync();
            if (request == null)
            {
                return null;
            }
            else
            {
                return request;
            }

        }

        //login user using email and password
        public async Task<User?> LoginUser(LoginUserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return null;
            }
            else
            {
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
                if(result == PasswordVerificationResult.Failed){
                    return null;
                }else{
                    return user;
                }
            }


        }


    }



}
