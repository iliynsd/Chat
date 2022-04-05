using System;

namespace Chat.Models
{
    public class ChatAction
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }
        public string ActionText { get; set; }

        public ChatAction() { }
        public ChatAction(string text)
        {
            Time = DateTime.Now;
            ActionText = text;
        }
    }
}