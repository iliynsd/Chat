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

        public void Delete(Chat chat) => _dataContext.Chats.FirstOrDefault(i => i.Id == chat.Id).IsActive = false;

        public List<Chat> GetAll() => _dataContext.Chats.Where(i => i.IsActive).ToList();

        public Chat GetChat(string chatName) => _dataContext.Chats.Where(i => i.IsActive).FirstOrDefault(i => i.Name == chatName);

        public void SaveToDb() => _dataContext.SaveChanges();
    }
}
