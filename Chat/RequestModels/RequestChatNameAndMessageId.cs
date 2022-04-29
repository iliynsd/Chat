using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.RequestModels
{
    public class RequestChatNameAndMessageId
    {
        public string ChatName { get; set; }
        public int MessageId { get; set; }
    }
}
