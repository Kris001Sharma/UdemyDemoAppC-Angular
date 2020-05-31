using System;
using System.Threading.Tasks;
using DemoApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context){
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {     
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null){
                return null;
            }

            if (!VerifyPasswordHash(password, user.PassHash, user.PassSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passHash, byte[] passSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++){
                    if(computedHash[i] != passHash[i])
                        return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte [] passwordHash, passwordSalt;
            // pass by refrence 
            CreatePasswordHash(password, out passwordHash, out passwordSalt);   

            user.PassHash = passwordHash;
            user.PassSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // sha encrptry implements IDisposable
            // to ensure dispose method is called when we are finished with the class            
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // anything here is disposed as soon as the block finishes
                passwordSalt = hmac.Key; // HMAC provides us random key
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); 
                    // ComputeHash takes byte array not string, so the encoding
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username == username)){
                return true;
            }
            return false;
        }
    }
}