using Chat.RequestModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Chat
{
    public static class RequestParser
    {
        public static T ParsePostRequest<T>(Stream request)
        {
            using (var reader = new StreamReader(request))
            {
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public static RequestUserName ParseGetRequestUserName(string url)
        {
            var param = new RequestUserName();
            param.UserName = url.Split('=').LastOrDefault();
            return param;
        }

        public static RequestChatName ParseGetRequestChatName(string url)
        {
            var param = new RequestChatName();
            param.ChatName = url.Split('=').LastOrDefault();
            return param;
        }

        public static RequestChatNameAndMessageId ParseGetRequestChatNameAndMessageId(string url)
        {
            var result = new RequestChatNameAndMessageId();
            var parameters = url.Split('&');
            result.ChatName = parameters?[0].Split('=')?[1];
            result.MessageId = Convert.ToInt32(parameters?[1].Split('=')?[1]);
            return result;
        }
    }
}