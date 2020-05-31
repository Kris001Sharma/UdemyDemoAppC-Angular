using System.Threading.Tasks;
using DemoApp.API.Models;

namespace DemoApp.API.Data
{
    public interface IAuthRepository
    {
        // 1. register user 
        // 2. login 
        // 3. user exists?  unique user names

        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}