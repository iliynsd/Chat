using System;
using System.Collections.Generic;

namespace Chat.Repositories
{
    public interface IChatActionsRepository
    {
        public void Add(Action action);
        public List<Action> Get();
        public void SaveToDb();
     }
}