using Chat.Models;
using System.Collections.Generic;

namespace Chat.Repositories
{
    public interface IChatActionsRepository
    {
        public void Add(ChatAction action);
        public List<ChatAction> GetAll();
        public ChatAction Get(string chatActionText);
        public void SaveToDb();
    }
}