using System.Collections.Generic;

namespace Chat.Repositories
{
    public interface IChatActionsRepository
    {
        public void Add(string action);
        public List<string> Get();
        public void SaveToDb();
        public void GetFromDb();
    }
}