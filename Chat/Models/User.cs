

using System.Collections.Generic;

namespace Chat
{
    public class User
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public User(string name, string type, bool isActive)
        {
            Name = name;
            Type = type;
            IsActive = isActive;
            Chats = new List<Chat>();
        }
    }
}