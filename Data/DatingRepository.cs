using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<User> GetUser(int id)
        {
           return await _context.Users.Include(p =>p.Photos).FirstOrDefaultAsync(x=>x.Id==id);    

        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.Include(p =>p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;   
        }
    }
}