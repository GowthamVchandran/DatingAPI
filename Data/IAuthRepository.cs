using System.Threading.Tasks;
using DatingAPI.Model;

namespace DatingAPI.Data
{
    public interface IAuthRepository
    {
        Task<User> Register (User UserName,string Password);
        Task<User> Login (string UserName,string Password);   
         Task<bool> IsExists (string UserName);   
    }
}