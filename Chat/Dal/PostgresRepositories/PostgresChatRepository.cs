using Chat.Dal;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Repositories.PostgresRepositories
{
    public class PostgresChatRepository : IChatRepository
    {
        private DataContext _dataContext;

        public PostgresChatRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(Chat chat) => _dataContext.Chats.Add(chat);

        public void Delete(Chat chat) => _dataContext.Chats.Find(chat).IsActive = false;

        public List<Chat> GetAll() => _dataContext.Chats.ToList();

        public Chat GetChat(string chatName) => _dataContext.Chats.Find(chatName);

        public void GetFromDb()
        { }

        public void SaveToDb() => _dataContext.SaveChanges();
    }
}
