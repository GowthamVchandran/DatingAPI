using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAPI.Controllers.Helper;
using DatingAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Data
{
    public class DatingRepository : IDatingRepository
    {
        private  readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context=context;
        }
        public  void Add<T>(T entity) where T : class
        {
             _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhoto(int id)
        {
            return await _context.Photos.Where(x => x.UserId == id).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _context.Photos.FirstOrDefaultAsync(x =>x.Id == id);
        }
    
        public async Task<PageList<User>> GetUsers(userParams userParams)
        {
           var UserResult =  _context.Users.Include(p =>p.Photos).OrderByDescending(x=>x.LastActive)
           .AsQueryable();  

           UserResult = UserResult.Where(x =>x.Id!= userParams.UserId);

           UserResult = UserResult.Where(x =>x.Gender == userParams.Gender);

           if(userParams.Likers)
           {
                var userlikers = await GetUsersLikes(userParams.UserId,userParams.Likers);
                UserResult = UserResult.Where(x=> userlikers.Contains(x.Id));
           }

           if(userParams.Likees)
           {
               
                var userlikees = await GetUsersLikes(userParams.UserId,userParams.Likers);
                UserResult = UserResult.Where(x=> userlikees.Contains(x.Id));
           }

          if(userParams.MinAge != 18 || userParams.MaxAge != 99){

           var maxDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
           var minDob = DateTime.Today.AddYears(-userParams.MinAge);
            
           UserResult =  UserResult.Where(x => x.DOB>=minDob && x.DOB <= maxDob );
         }

         if(!string.IsNullOrEmpty(userParams.Orderby))
         {
             switch (userParams.Orderby)
             {
                 case "created":
                     UserResult = UserResult.OrderByDescending(x=>x.Createed);
                     break;
                     
                 default:
                      UserResult = UserResult.OrderByDescending(x =>x.LastActive);
                      break;
             }
         }
           return await PageList<User>.CreateAsync(UserResult, userParams.PageNumber,userParams.PageSize);
    }

        public async Task<User> GetUser(int Id)
        {
            return await _context.Users.Include(p =>p.Photos).FirstOrDefaultAsync(x =>x.Id == Id);
            
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync()>0; 
        }

        public async Task<Likes> GetLike(int UserId, int RecipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(x => x.LikerID == UserId && x.LikeeID == RecipientId);
        }

        private async Task<IEnumerable<int>> GetUsersLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(x => x.Likers).Include(x => x.Likees)
            .FirstOrDefaultAsync(x => x.Id==id);

            if(likers)
            {
                return user.Likers.Where(x=> x.LikeeID==id).Select(i => i.LikerID);
            }
            else
            {
                return user.Likees.Where(x=> x.LikerID==id).Select(i => i.LikeeID);
            }
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(x =>x.Id == id);
        }

        public async Task<PageList<Message>> GetMessageForUser(MessageParams msgParams)
        {
            var msg = _context.Messages.Include(x => x.Sender).ThenInclude(p => p.Photos)
                          .Include(x => x.Recipient).ThenInclude(p => p.Photos).AsQueryable();

                switch (msgParams.MessageContainer)
                {
                    case "Inbox":
                             msg = msg.Where(x=>x.RecipientId == msgParams.UserId && 
                              x.RecipientDeleted == false);
                             break;
                    case "Outbox":
                             msg = msg.Where(x=>x.SenderId == msgParams.UserId && 
                              x.SenderDeleted == false);
                             break;
                    default:
                             msg = msg.Where(x=>x.RecipientId == msgParams.UserId 
                             && x.RecipientDeleted == false && x.IsRead == false);
                             break;
                }
            msg = msg.OrderByDescending(x=>x.MessageSent);
            return await PageList<Message>.CreateAsync(msg, msgParams.PageNumber, msgParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
                  var msg = await _context.Messages.Include(x => x.Sender).ThenInclude(p => p.Photos)
                          .Include(x => x.Recipient).ThenInclude(p => p.Photos)
                          .Where( m => m.RecipientId == userId && m.RecipientDeleted == false &&
                           m.SenderId == recipientId ||
                          m.RecipientId == recipientId && m.SenderId == userId 
                          && m.SenderDeleted == false)
                        .OrderByDescending(x=>x.MessageSent)
                        .ToListAsync(); 

                    return msg;
        }
    }
}