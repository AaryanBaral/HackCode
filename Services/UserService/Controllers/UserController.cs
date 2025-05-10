using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using UserService.Data;
using UserService.Entities;
// using UserService.Interfaces;
using UserService.Mappers;
using UserService.Models;
// using UserService.Repositories;
using UserService.services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;

        // private readonly IConfiguration _configuration;

        public UserController(UserDbContext context, UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {

            _context = context;
            _signInManager = signInManager;
            // _configuration = Configuration;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]

        public async Task<ActionResult<User>> RegisterUser(RegisterUserDto register)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.Users
       .FirstOrDefaultAsync(u => u.Email == register.Email);

            if (existingUser != null)
                return Conflict("User with the given email already exists.");

            var user = new User
            {
                UserName = register.UserName,
                Email = register.Email,
                CreatedOn = DateTime.UtcNow

            };
            var createdUser = await _userManager.CreateAsync(user, register.Password);
            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (roleResult.Succeeded)
                {
                    return CreatedAtAction(nameof(RegisterUser), new { id = user.Id }, user);
                }
                else
                {
                    throw new InvalidOperationException("Failed to assign role");
                }
            }
            else
            {

                throw new InvalidOperationException("user creation failed");
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<newUserDto>> LoginUser(LoginUserDto login)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null)
            {
                return BadRequest("");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Username not found or incorrect password");
            }
            ;
            return Ok(
             new newUserDto
             {
                 UserName = user.UserName,
                 Email = user.Email,
                 Token = _tokenService.CreateToken(user)
             }
            );
        }


        // public String CreateToken(User user)
        // {
        //     var claims = new List<Claim>{
        //         new Claim(ClaimTypes.Name,user.UserName),

        //     };
        //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<String>("AppSettings:token")!));
        //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        //     var tokenDescriptor = new JwtSecurityToken(
        //        issuer: _configuration.GetValue<String>("AppSettings:issuer"),
        //        audience: _configuration.GetValue<String>("AppSettings:audience"),
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(10),
        //        signingCredentials: creds
        //     );
        //     return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);





        // }
    }
}