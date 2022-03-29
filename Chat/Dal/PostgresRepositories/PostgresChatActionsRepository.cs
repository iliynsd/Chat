using Chat.Dal;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Repositories.PostgresRepositories
{
    public class PostgresChatActionsRepository : IChatActionsRepository
    {
        private DataContext _dataContext;

        public PostgresChatActionsRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(string action) => _dataContext.ChatActions.Add(action);

        public List<string> Get() => _dataContext.ChatActions.ToList();

        public void GetFromDb()
        { }

        public void SaveToDb() => _dataContext.SaveChanges();
    }
}