using Chat.Dal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Repositories.PostgresRepositories
{
    public class PostgresMessageRepository : IMessageRepository
    {
        private DataContext _dataContext;

        public PostgresMessageRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(Message message) => _dataContext.Messages.Add(message);

        public void Delete(Message message) => _dataContext.Messages.Find(message).IsActive = false;

        public List<Message> GetAll() => _dataContext.Messages.ToList();

        public List<Message> GetChatMessages(Chat chat) => _dataContext.Messages.ToList().FindAll(i => i.IsActive).FindAll(i => i.ChatId == chat.Id);

        public void GetFromDb()
        { }

        public bool IsChatNotEmpty(Chat chat) => _dataContext.Messages.Select(i => i.ChatId).Contains(chat.Id);

        public void SaveToDb() => _dataContext.SaveChanges();
    }
}
