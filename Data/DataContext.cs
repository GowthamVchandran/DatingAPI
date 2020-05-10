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
            optionsBuilder.UseSqlServer("Data Source=USER-PC\\SQLEXPRESS;Initial Catalog=student;Integrated Security=True",
            builder =>builder.UseRowNumberForPaging());
        }
        public DataContext()
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserValue> UserValues { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Likes> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Likes>().HasKey(k => new {k.LikerID,k.LikeeID});

            builder.Entity<Likes>().HasOne(x => x.Likee).WithMany(u => u.Likers)
               .HasForeignKey(x => x.LikeeID).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Likes>().HasOne(x => x.Liker).WithMany(u => u.Likees)
               .HasForeignKey(x => x.LikerID).OnDelete(DeleteBehavior.Restrict);

             builder.Entity<Message>().HasOne(x => x.Sender).WithMany(u => u.MessageSent)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>().HasOne(x => x.Recipient).WithMany(u => u.MessageReceived)
              .OnDelete(DeleteBehavior.Restrict);
        }
            
    }
}
