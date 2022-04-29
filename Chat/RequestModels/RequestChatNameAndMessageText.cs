using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.RequestModels
{
    public class RequestChatNameAndMessageText
    {
        [JsonProperty("chatName")]
        public string ChatName { get; set; }
        [JsonProperty("messageText")]
        public string MessageText { get; set; }
    }
}
