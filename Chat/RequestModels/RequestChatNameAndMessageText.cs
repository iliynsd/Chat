using Newtonsoft.Json;

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
