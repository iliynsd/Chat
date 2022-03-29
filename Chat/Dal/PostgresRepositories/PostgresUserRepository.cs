﻿using Chat.Dal;
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

        public void Delete(User user) => _dataContext.Users.Find(user).IsActive = false;

        public User Get(string userName) => _dataContext.Users.Find(userName);

        public List<User> GetAll() => _dataContext.Users.ToList();

        public void GetFromDb()
        {

        }

        public bool IsUserExist(string username) => _dataContext.Users.Select(i => i.Name).Contains(username);

        public void SaveToDb() => _dataContext.SaveChanges();

        public bool UserHasChats(string userName) => _dataContext.Users.Select(i => i).Where(i => i.IsActive == true).First(i => i.Name == userName).ChatIds.Count > 0;
    }
}
