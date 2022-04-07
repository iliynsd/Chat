using System.Collections.Generic;

namespace Chat.Repositories
{
    public interface IChatRepository
    {
        public void Add(Chat chat);
        public void Delete(Chat chat);
        public List<Chat> GetAll();
        public void Save();

        public Chat GetChat(string chatName);
        public Chat GetChatById(int id);
    }
}