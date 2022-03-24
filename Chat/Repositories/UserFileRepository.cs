using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chat.Repositories
{
    public class UserFileRepository : FileBaseRepository<UserFileRepository>, IUserRepository
    {
        private List<User> _users;

        public UserFileRepository()
        {
            _users = new List<User>();
        }
        
        public void Add(User user) => _users.Add(user);

        public void Delete(User user) => _users.Find(i => i.Id == user.Id).IsActive = false;

        public User Get(string userName) => _users.FindAll(i => i.IsActive).Find(i => i.Name == userName);

        public List<User> GetAll() =>  _users.FindAll(i => i.IsActive);

        public void SaveToDb(string source) => SaveToFile(this, source);

        public void GetFromDb(string source) => _users = GetFromFile(source).GetAll();
        
        protected override void Write(BinaryWriter writer, UserFileRepository userFileRepository)
        {
            foreach (var user in userFileRepository.GetAll())
            {
                if (!string.IsNullOrEmpty(user.Name))
                {
                    writer.Write(user.Id);
                    writer.Write(user.Type);
                    string chatIds = string.Empty;
                    if(user.ChatIds.Count<1)
                    {
                        writer.Write("0;");
                    }
                    else
                    {
                        foreach (var id in user.ChatIds)
                        {
                          chatIds += id;
                          chatIds += ';';
                         }
                         writer.Write(chatIds);
                    }
                    
                    
                    writer.Write(user.Name);
                    writer.Write(user.IsActive);
                }
            }
        }

        protected override UserFileRepository Read(BinaryReader reader)
        {
            var users = new UserFileRepository();
            
            while(reader.PeekChar() > -1)
            {
                var id = reader.ReadInt32();
                var type = reader.ReadString();

                var chatIdsMas = reader.ReadString().Split(';').ToList();
                chatIdsMas.Remove(chatIdsMas.Last());
                var chatIds = chatIdsMas.Select(i => int.Parse(i)).ToList();
                var name = reader.ReadString();
                var isActive = reader.ReadBoolean();
                
                if (!string.IsNullOrEmpty(name))
                {
                    users.Add(new User() 
                    {
                        Id = id,
                        Type = type,
                        Name = name,
                        IsActive = isActive,
                        ChatIds = chatIds
                    });
                }
            }
            
            return users;
        }
    }
}