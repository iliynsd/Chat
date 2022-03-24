using System.Collections.Generic;

namespace Chat.Repositories
{
    public interface IUserRepository
    {
        public void Add(User user);
        public void Delete(User user);
        public User Get(string userName);
        public List<User> GetAll();
        public void SaveToDb(string source);
        public void GetFromDb(string source);
    }
}