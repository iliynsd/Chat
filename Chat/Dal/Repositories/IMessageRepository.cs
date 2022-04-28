using System.Collections.Generic;

namespace Chat.Repositories
{
    public interface IMessageRepository
    {
        public void Add(Message message);
        public bool Delete(Message message);
        public List<Message> GetAll();
        public void Save();

        public List<Message> GetChatMessages(Chat chat);
        public bool IsChatNotEmpty(Chat chat);

    }
}