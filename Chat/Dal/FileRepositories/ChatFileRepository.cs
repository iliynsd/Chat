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

        public void Add(Chat chat)
        {
            chat.Id = 1;
            if (_chats.Count() > 0)
            {
                chat.Id = _chats.Last().Id++;
            }

            _chats.Add(chat);
        }

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
                    string users = String.Empty; 
                    foreach (var user in chat.Users)
                    {
                        users += user.Id;
                        users += ';';
                    }

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
                var usersMas = reader.ReadString().Split(';').ToList();
                usersMas.Remove(usersMas.Last());
                

                //TODO read list users from file
                var isActive = reader.ReadBoolean();

                if (!string.IsNullOrEmpty(name))
                {
                    chats.Add(new Chat()
                    {
                        Id = id,
                        Name = name,
                        Users = users,
                        IsActive = isActive
                    });
                }
            }

            return chats;
        }

        public Chat GetChat(string chatName) => _chats.FindAll(i => i.IsActive).Find(i => i.Name == chatName);
    }
}