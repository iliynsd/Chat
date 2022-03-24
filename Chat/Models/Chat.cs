using System.Collections.Generic;

namespace Chat
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> UserIds { get; set; }
        public bool IsActive { get; set; }
    }
}