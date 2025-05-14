using UserService.Model;
using System.Threading.Tasks;
using UserService.Entities;

namespace UserService.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> CreateUserAsync(User user, string password);
        Task AddToRoleAsync(User user, string role);
        Task<bool> CheckPasswordAsync(User user, string password, bool lockoutOnFailure = false);
        Task<bool> ValidateUserId(string userId);
        Task<List<string>> GetAllUserIds();
    }
}
