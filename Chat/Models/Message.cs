using System;

namespace Chat
{
    public class Message
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ChatId { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public bool IsViewed { get; set; }
        public bool IsActive { get; set; }

        public Message() { }
    }
}