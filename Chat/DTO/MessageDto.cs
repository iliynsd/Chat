using System;

namespace Chat.DTO
{
    public class MessageDto
    {
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public bool IsViewed { get; set; }
    }
}
