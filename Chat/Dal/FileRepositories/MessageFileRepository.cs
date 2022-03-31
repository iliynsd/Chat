using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chat.Repositories
{
    public class MessageFileRepository : FileBaseRepository<List<Message>>, IMessageRepository
    {
        private List<Message> _messages;
        private Options _options;
        public MessageFileRepository(IOptions<Options> options)
        {
            _messages = new List<Message>();
            _options = options.Value;
        }

        public void Add(Message message)
        {
            message.Id = 1;
            if (_messages.Count() > 0)
            {
                message.Id = _messages.Last().Id++;
            }

            _messages.Add(message);
        } 

        public void Delete(Message message) => _messages.FindAll(i => i.ChatId == message.ChatId).Find(i => i.Id == message.Id).IsActive = false;

        public List<Message> GetAll() => _messages.FindAll(i => i.IsActive);

        public void SaveToDb() => SaveToFile(_messages, _options.PathToMessages);

        public void GetFromDb() => _messages = GetFromFile(_options.PathToMessages);

        protected override void Write(BinaryWriter writer, List<Message> item)
        {
            foreach (var message in item)
            {
                if (!string.IsNullOrEmpty(message.Text))
                {
                    writer.Write(message.Id);
                    writer.Write(message.UserId);
                    writer.Write(message.ChatId);
                    writer.Write(message.Text);
                    writer.Write(message.Time.ToString("t"));
                    writer.Write(message.IsViewed);
                    writer.Write(message.IsActive);
                }
            }
        }

        protected override List<Message> Read(BinaryReader reader)
        {
            var messages = new List<Message>();

            while (reader.PeekChar() > -1)
            {
                var message = new Message()
                {
                    Id = reader.ReadInt32(),
                    UserId = reader.ReadInt32(),
                    ChatId = reader.ReadInt32(),
                    Text = reader.ReadString(),
                    Time = Convert.ToDateTime(reader.ReadString()),
                    IsViewed = reader.ReadBoolean(),
                    IsActive = reader.ReadBoolean()
                };

                if (!string.IsNullOrEmpty(message.Text))
                {
                    messages.Add(message);
                }
            }

            return messages;
        }

        public List<Message> GetChatMessages(Chat chat) => _messages.FindAll(i => i.IsActive).FindAll(i => i.ChatId == chat.Id);
        public bool IsChatNotEmpty(Chat chat) => _messages.Select(i => i.ChatId).Contains(chat.Id);
    }
}