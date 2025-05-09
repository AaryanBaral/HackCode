
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Data;
using UserService.Entities;
using UserService.Interfaces;
using UserService.Mappers;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IUserServiceRepository _userRepo;
        private readonly IConfiguration _configuration;

        public UserController(UserDbContext context, IUserServiceRepository userRepo, IConfiguration Configuration)
        {
            
            _context = context;
            _userRepo = userRepo;
            _configuration = Configuration;
        }

        [HttpPost("register")]

        public async Task<ActionResult<User>> RegisterUser(RegisterUserDto register)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
            if (existingUser != null)
            {
                return BadRequest("User Already Exists");
            }

            var Hasher = new PasswordHasher<User>();
            var newUser = register.ToRegisterUser(Hasher);
            await _userRepo.RegisterUser(newUser);

            if (newUser == null)
            {
                return BadRequest("no user detail provided");
            }
            else
            {
                return CreatedAtAction(nameof(RegisterUser), new { id = newUser.Id }, newUser);
            }



        }
        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser(LoginUserDto login)
        {
            var user = await _userRepo.LoginUser(login);
            if (user == null)
            {
                return BadRequest("Wrong Password");
            }
            String token = CreateToken(user);
            return Ok(token);
        }


        public String CreateToken(User user)
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name,user.UserName),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<String>("AppSettings:token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
               issuer: _configuration.GetValue<String>("AppSettings:issuer"),
               audience: _configuration.GetValue<String>("AppSettings:audience"),
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(10),
               signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);





        }
    }
}