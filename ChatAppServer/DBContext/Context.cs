using ChatAppServer.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ChatAppServer.DBContext
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB Database=ChatAppUsers Trusted_Connection=True");
        }
    }

}
