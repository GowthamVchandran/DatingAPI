using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DbContext> options) : base(options)
        {

        }
         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=USER-PC\\SQLEXPRESS;Initial Catalog=student;Integrated Security=True");
        }
        public DataContext()
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserValue> UserValues { get; set; }
    }
}
