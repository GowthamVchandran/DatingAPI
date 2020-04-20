using System.Collections.Generic;
using System.Linq;
using DatingAPI.Model;
using Newtonsoft.Json;

namespace DatingAPI.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context)
        {
           if(!context.Users.Any())
           {
             var userData =  System.IO.File.ReadAllText("Data/UserSeedData.json");
             var user = JsonConvert.DeserializeObject<List<User>>(userData);

             foreach (var item in user)
             {
                 byte []  passwordhash,passwordSalt;
                 createPassword("testpassword",out passwordhash,out passwordSalt);

                item.PasswordHash =passwordhash;
                item.PasswordSalt= passwordSalt;
                item.UserName = item.UserName.ToLower();
                context.Users.Add(item);
             }
             context.SaveChanges();
           }
        }

        
        private static void createPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using( var hash= new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hash.Key;
                passwordHash = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }
    }
}