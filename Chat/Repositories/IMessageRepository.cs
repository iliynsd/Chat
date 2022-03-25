using System.Collections.Generic;

namespace Chat.Repositories
{
    public interface IMessageRepository
    {
        public void Add(Message message);
        public void Delete(Message message);
        public List<Message> GetAll();
        public void SaveToDb(string source);
        public void GetFromDb(string source);

        public List<Message> GetChatMessages(Chat chat);
    }
}