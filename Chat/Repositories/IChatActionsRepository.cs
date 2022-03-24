using System.Collections.Generic;

namespace Chat.Repositories
{
    public interface IChatActionsRepository
    {
        public void Add(string action);
        public List<string> Get();
        public void SaveToDb(string source);
        public void GetFromDb(string source);
    }
}