using System;
using System.Threading.Tasks;
using DatingAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Data
{
    public class AuthReposity : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthReposity(DataContext context)
        {
            _context=context;
        }
        public async Task<bool> IsExists(string UserName)
        {           
            return  await _context.Users.AnyAsync(x=>x.UserName == UserName);
        }

        public async Task<User> Login(string UserName, string Password)
        {
            var User = await _context.Users.Include(x =>x.Photos).FirstOrDefaultAsync(x=>x.UserName == UserName);

            if(User==null)
            return null;

            if(!verifyPassword(Password,User.PasswordHash,User.PasswordSalt))
               return null;

            return User;
        }

        private bool verifyPassword(string password,byte [] PasswordHash,byte [] PasswordSalt)
        {
            using( var hash= new System.Security.Cryptography.HMACSHA512(PasswordSalt))
            {
             var Generate_password = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

              for(int i=0;i< Generate_password.Length;i++){
                  if(PasswordHash[i]!=Generate_password[i])
                  return false;
              }
            }

            return true;           
        }

        public async Task<User> Register(User User, string Password)
        {
            byte [] PasswordHash, PasswordSalt;

            createPassword(Password,out PasswordHash,out PasswordSalt);
            User.PasswordHash = PasswordHash;
            User.PasswordSalt= PasswordSalt;

            await _context.Users.AddAsync(User);
            await _context.SaveChangesAsync();

            return User;
        }

        private void createPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using( var hash= new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hash.Key;
                passwordHash = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }
    }
}