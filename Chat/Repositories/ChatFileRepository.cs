using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chat.Repositories
{
    public class ChatFileRepository : FileBaseRepository<List<Chat>>, IChatRepository
    {
        private List<Chat> _chats;
        private Options _options;
        public ChatFileRepository(IOptions<Options> options)
        {
            _chats = new List<Chat>();
            _options = options.Value;
        }

        public void Add(Chat chat) => _chats.Add(chat);

        public void Delete(Chat chat) => _chats.Find(i => i.Id == chat.Id).IsActive = false;

        public List<Chat> GetAll() => _chats.FindAll(i => i.IsActive);

        public void SaveToDb() => SaveToFile(_chats, _options.PathToChats);

        public void GetFromDb() => _chats = GetFromFile(_options.PathToChats);

        protected override void Write(BinaryWriter writer, List<Chat> chats)
        {
            foreach (var chat in chats)
            {
                if (!string.IsNullOrEmpty(chat.Name))
                {
                    writer.Write(chat.Id);
                    writer.Write(chat.Name);
                    string userIds = String.Empty;
                    foreach (var id in chat.UserIds)
                    {
                        userIds += id;
                        userIds += ';';
                    }
                    writer.Write(userIds);
                    writer.Write(chat.IsActive);
                }
            }
        }

        protected override List<Chat> Read(BinaryReader reader)
        {
            var chats = new List<Chat>();
            while (reader.PeekChar() > -1)
            {

                var id = reader.ReadInt32();
                var name = reader.ReadString();
                var userIdsMas = reader.ReadString().Split(';').ToList();
                userIdsMas.Remove(userIdsMas.Last());
                var userIds = userIdsMas.Select(i => int.Parse(i)).ToList();
                var isActive = reader.ReadBoolean();

                if (!string.IsNullOrEmpty(name))
                {
                    chats.Add(new Chat()
                    {
                        Id = id,
                        Name = name,
                        UserIds = userIds,
                        IsActive = isActive
                    });
                }
            }

            return chats;
        }

        public Chat GetChat(string chatName) => _chats.FindAll(i => i.IsActive).Find(i => i.Name == chatName);
    }
}