using System;

namespace Chat.Models
{
    public class Action
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Time { get; set; }
        public string ActionText { get; set; }
    }
}