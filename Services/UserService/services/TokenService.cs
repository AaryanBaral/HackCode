using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using UserService.Entities;

namespace UserService.services
{
    //creating a token
    //1 list claims
    // 2 get signing credentials from symmetricsecuritykey 
    // 3 add token Description in tokenDescriptor 
    // 4 generate token using tooken handler
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration configuration){
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<String>("AppSettings:token")));

        }
        
        public string CreateToken(User user)
        {
            var claims =new List<Claim>{
                new (ClaimTypes.Name,user.UserName ?? String.Empty),
                new(ClaimTypes.Email,user.Email ?? String.Empty)       
            };
            var creds = new SigningCredentials(_key,SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
               issuer: _configuration.GetValue<String>("AppSettings:issuer"),
               audience: _configuration.GetValue<String>("AppSettings:audience"),
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(10),
               signingCredentials: creds
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenDescriptor);
                return token;
        }
    }
}