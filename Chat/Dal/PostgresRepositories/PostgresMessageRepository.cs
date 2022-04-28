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

        public bool Delete(Message message)
        {
            var mes = _dataContext.Messages.FirstOrDefault(i => i.Id == message.Id);
            if (mes != null)
            {
                mes.IsActive = false;
                return true;
            }
            return false;
        } 

        public List<Message> GetAll() => _dataContext.Messages.Where(i => i.IsActive).ToList();

        public List<Message> GetChatMessages(Chat chat) => _dataContext.Messages.Where(i => i.IsActive).Where(i => i.ChatId == chat.Id).ToList();


        public bool IsChatNotEmpty(Chat chat) => _dataContext.Messages.Select(i => i.ChatId).Contains(chat.Id);


        public void Save() => _dataContext.SaveChanges();
    }
}