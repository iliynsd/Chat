using System.Collections.Generic;

namespace Chat
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public Chat()
        {
            Users = new List<User>();
        }
    }
}