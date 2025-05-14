using Microsoft.AspNetCore.Identity;
using UserService.Data;
using UserService.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserService.Entities;
using UserService.Repositories;

namespace UserService.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthRepository(UserManager<User> userManager,SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded ? user : null;
        }

        public async Task AddToRoleAsync(User user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password,bool lockoutOnFailure = false)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<List<string>> GetAllUserIds(){
            return await _userManager.Users
            .Select(u => u.Id)
            .ToListAsync();
        }
    }
}
