using UserService.Repositories;
using UserService.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using UserService.Entities;
using UserService.Services.AuthenticationService;
using UserService.Mappers;
using UserService.Services.JWTService;


namespace UserService.Services.AuthenticationService
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _config;

        private readonly IJWTService _tokenService;
        public AuthService(
        IAuthRepository authRepository,
        IConfiguration config,
        IJWTService tokenService
        )
        {
            _authRepository = authRepository;
            _config = config;
            _tokenService = tokenService;
        }
        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            // Check if user already exists by email
            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already exists." });

            }
            var user = UserMapper.ToRegisterUser(dto);
            var createdUser = await _authRepository.CreateUserAsync(user, dto.Password);

            if (createdUser == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Username: " + user.UserName + " Email: " + user.Email + " Password: " + dto.Password });
            }

            await _authRepository.AddToRoleAsync(createdUser, "User");
            return IdentityResult.Success;
        }
        public async Task<AuthResponseDto?>LoginAsync(LoginDto dto)
        {
            var user = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (user == null || !await _authRepository.CheckPasswordAsync(user, dto.Password))
                return null;
            var token = _tokenService.GenerateJwtToken(user); 

            return new AuthResponseDto
            {
                Token = token,
                Role ="User"
            };
        }
    }
    }

