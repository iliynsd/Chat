using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace Chat.Repositories
{
    public class ChatActionsFileRepository : FileBaseRepository<List<string>>, IChatActionsRepository
    {
        private List<string> _actions;
        private Options _options;

        public ChatActionsFileRepository(IOptions<Options> options)
        {
            _actions = new List<string>();
            _options = options.Value;
        }

        public void Add(string action) => _actions.Add(action);

        public List<string> Get() => _actions;

        public void SaveToDb() => SaveToFile(_actions, _options.PathToChatActions);

        public void GetFromDb() => _actions = GetFromFile(_options.PathToChatActions);

        protected override void Write(BinaryWriter writer, List<string> chatActions) => chatActions.ForEach(writer.Write);

        protected override List<string> Read(BinaryReader reader)
        {
            var actions = new List<string>();

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