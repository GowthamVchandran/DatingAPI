using System.Collections.Generic;
using System.Threading.Tasks;
using DatingAPI.Controllers.Helper;
using DatingAPI.Model;

namespace DatingAPI.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T:class;
        void Delete<T>(T entity) where T:class;
        Task<bool> SaveAll();
        Task<PageList<User>> GetUsers(userParams userParams);
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhoto(int id);
        Task<Likes> GetLike(int UserId, int RecipientId);
        Task<Message> GetMessage(int id);
        Task<PageList<Message>> GetMessageForUser(MessageParams id);
        Task<IEnumerable<Message>> GetMessageThread(int id,int RecipientId) ;
    }
}