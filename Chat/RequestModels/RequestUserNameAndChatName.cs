using Newtonsoft.Json;

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
