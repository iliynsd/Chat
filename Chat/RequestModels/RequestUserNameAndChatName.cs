using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.RequestModels
{
    public class RequestUserNameAndChatName
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("chatName")]
        public string ChatName { get; set; }
    }
}
