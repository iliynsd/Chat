using Microsoft.EntityFrameworkCore;
using System;

namespace Chat.Dal
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Action> ChatActions { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
           
        }
    }
}
