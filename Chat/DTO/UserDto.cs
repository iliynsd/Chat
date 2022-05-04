using System.Collections.Generic;

namespace Chat.DTO
{
    public class UserDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public IList<ChatDto> Chats { get; set; }
    }
}
