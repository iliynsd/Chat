using System;
using System.Collections.Generic;
using System.IO;

namespace Chat.Repositories
{
    public class MessageFileRepository : FileBaseRepository<MessageFileRepository>, IMessageRepository
    {
        private List<Message> _messages;

        public MessageFileRepository()
        {
            _messages = new List<Message>();
        }
        
        public void Add(Message message) => _messages.Add(message);

        public void Delete(Message message) => _messages.FindAll(i => i.ChatId == message.ChatId).Find(i => i.Id == message.Id).IsActive = false;
        
        public List<Message> GetAll() => _messages.FindAll(i => i.IsActive);

        public void SaveToDb(string source) => SaveToFile(this, source);

        public void GetFromDb(string source) => _messages = GetFromFile(source).GetAll();
        
        protected override void Write(BinaryWriter writer, MessageFileRepository item)
        {
            foreach (var message in item.GetAll())
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

        protected override MessageFileRepository Read(BinaryReader reader)
        {
            var messages = new MessageFileRepository();

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
    }
}