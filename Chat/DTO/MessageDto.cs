using System;

namespace Chat.DTO
{
    public class MessageDto
    {
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public bool IsViewed { get; set; }
    }
}
