using System.Collections.Generic;

namespace Chat
{
    public class User
    {
        public int Id { get; set; }
        public string Type { get; set; }

        public List<int> ChatIds { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}