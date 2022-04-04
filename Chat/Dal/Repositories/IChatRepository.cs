using System.Collections.Generic;

namespace Chat.Repositories
{
    public interface IChatRepository
    {
        public void Add(Chat chat);
        public void Delete(Chat chat);
        public List<Chat> GetAll();
        public void SaveToDb();

        public Chat GetChat(string chatName);
    }
}