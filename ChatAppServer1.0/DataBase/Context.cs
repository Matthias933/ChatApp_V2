using ChatAppServer1._0.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ChatAppServer1._0.DataBase
{
    public class Context : DbContext
    { 
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ChatAppUsers;Trusted_Connection=True;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
                .HasKey(c => c.ChatId);

            modelBuilder.Entity<Chat>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}


/*
 CREATE TABLE Users
(
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL
);

CREATE TABLE Chats
(
    ChatId INT PRIMARY KEY IDENTITY(1,1),
    SenderId INT FOREIGN KEY REFERENCES Users(UserId),
    ReceiverId INT FOREIGN KEY REFERENCES Users(UserId),
    Message NVARCHAR(MAX) NOT NULL,
    TimeStamp DATETIME NOT NULL
);
*/
