using Chat.Dal;
using Chat.Models;
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

        public void Add(ChatAction action) => _dataContext.ChatActions.Add(action);

        public List<ChatAction> GetAll() => _dataContext.ChatActions.ToList();

        public ChatAction Get(string chatActionText) => _dataContext.ChatActions.Where(i => i.ActionText == chatActionText).FirstOrDefault();

        public void Save() => _dataContext.SaveChanges();
    }
}