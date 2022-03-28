using System.Collections.Generic;
using System.IO;

namespace Chat.Repositories
{
    public class ChatActionsFileRepository : FileBaseRepository<ChatActionsFileRepository>, IChatActionsRepository
    {
        private List<string> _actions;

        public ChatActionsFileRepository()
        {
            _actions = new List<string>();
        }

        public void Add(string action) => _actions.Add(action);

        public List<string> Get() => _actions;

        public void SaveToDb(string source) => SaveToFile(this, source);

        public void GetFromDb(string source) => _actions = GetFromFile(source).Get();

        protected override void Write(BinaryWriter writer, ChatActionsFileRepository chatActions) => chatActions.Get().ForEach(writer.Write);

        protected override ChatActionsFileRepository Read(BinaryReader reader)
        {
            var actions = new ChatActionsFileRepository();

            while (reader.PeekChar() > -1)
            {
                var action = reader.ReadString();
                if (!string.IsNullOrEmpty(action))
                {
                    actions.Add(action);
                }
            }

            return actions;
        }
    }
}