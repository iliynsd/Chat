using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chat.Repositories
{
    public class UserFileRepository : FileBaseRepository<List<User>>, IUserRepository
    {
        private List<User> _users;
        private Options _options;

        public UserFileRepository(IOptions<Options> options)
        {
            _users = new List<User>();
            _options = options.Value;
        }

        public void Add(User user)
        {
            user.Id = 1;
            if (_users.Count > 0)
            {
                user.Id = _users.Last().Id++;
            }

            _users.Add(user);
        }

        public void Delete(User user) => _users.Find(i => i.Id == user.Id).IsActive = false;

        public User Get(string userName) => _users.FindAll(i => i.IsActive).Find(i => i.Name == userName);

        public List<User> GetAll() => _users.FindAll(i => i.IsActive);

        public bool IsUserExist(string username) => _users.Select(i => i.Name).Contains(username);

        public bool UserHasChats(string userName) => _users.FindAll(i => i.IsActive).Find(i => i.Name == userName).Chats.Count > 0;

        public void SaveToDb() => SaveToFile(_users, _options.PathToUsers);

        public void GetFromDb() => _users = GetFromFile(_options.PathToUsers);

        protected override void Write(BinaryWriter writer, List<User> users)
        {
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Name))
                {
                    writer.Write(user.Id);
                    writer.Write(user.Type);
                    string chatIds = string.Empty;
                    if (user.Chats.Count < 1)
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
                    }*/


                    writer.Write(user.Name);
                    writer.Write(user.IsActive);
                }
            }
        }

        protected override List<User> Read(BinaryReader reader)
        {
            var users = new List<User>();

            while (reader.PeekChar() > -1)
            {
                var id = reader.ReadInt32();
                var type = reader.ReadString();

                var chatIdsMas = reader.ReadString().Split(';').ToList();
                chatIdsMas.Remove(chatIdsMas.Last());
                var chatIds = chatIdsMas.Select(i => int.Parse(i)).ToList();
                var name = reader.ReadString();
                var isActive = reader.ReadBoolean();

                /* if (!string.IsNullOrEmpty(name))
                 {
                     users.Add(new User()
                     {
                         Id = id,
                         Type = type,
                         Name = name,
                         IsActive = isActive,
                         ChatIds = chatIds
                     });
                 }*/
            }

            return users;
        }
    }
}