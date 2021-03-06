using Chat.Dal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Repositories
{
    public class PostgresUserRepository : IUserRepository
    {
        private DataContext _dataContext;

        public PostgresUserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(User user) => _dataContext.Users.Add(user);

        public void Delete(User user) => _dataContext.Users.FirstOrDefault(i => i.Id == user.Id).IsActive = false;

        public User Get(string userName) => _dataContext.Users.FirstOrDefault(i => i.Name == userName);

        public List<User> GetAll() => _dataContext.Users.ToList();

        public User GetById(int userId) => _dataContext.Users.FirstOrDefault(i => i.Id == userId);

        public bool IsUserExist(string username) => _dataContext.Users.Select(i => i.Name).Contains(username);

        public void Save() => _dataContext.SaveChanges();


        public bool UserHasChats(string userName) => _dataContext.Users.Where(i => i.IsActive == true).First(i => i.Name == userName).Chats.Count > 0;
    }
}