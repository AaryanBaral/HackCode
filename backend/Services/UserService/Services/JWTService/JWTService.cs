using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using UserService.Entities;

namespace UserService.Services.JWTService
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;


        public JWTService(IConfiguration config)
        {
            _config = config;
#pragma warning disable CS8604 // Possible null reference argument.
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SigningKey"]));
#pragma warning restore CS8604 // Possible null reference argument.

        }
        public string GenerateJwtToken(User user)
        {


            var claims = new List<Claim>
            {
                new("UserName", user.UserName?.ToString() ?? string.Empty),
                new("UserId", user.Id.ToString() ?? string.Empty),
                new("Email", user.Email ?? string.Empty),

            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}