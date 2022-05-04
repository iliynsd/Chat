using System.Collections.Generic;

namespace Chat.DTO
{
    public class ChatDto
    {
        public string Name { get; set; }
        public IList<UserDto> Users { get; set; }
    }
}
