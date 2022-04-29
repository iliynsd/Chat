using Chat.RequestModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chat.Web
{
    public class RequestHandler : IHandler
    {
        private Messenger _messenger;
        private AppOptions.Options _options;
        
        private string RootRequestWithParamsTemplate(string method) => @$"/{method}?\w*";
        private string RootRequestTemplate(string method) => @$"/{method}";
        public RequestHandler(Messenger messenger, IOptions<AppOptions.Options> options)
        {
            _messenger = messenger;
            _options = options.Value;
        }
        //get
        private async Task SignInAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = RequestParser.ParseGetRequestUserName(request.RawUrl).UserName;
            var chats = _messenger.SignIn(userName)?.Select(i => i.Name);
            if (chats is not null)
            {
                var cookie = new Cookie("userName", userName);
                response.AppendCookie(cookie);
            }
            var answer = JsonConvert.SerializeObject(chats, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            await ResponseWriter.WriteResponseAsync(answer, response.OutputStream);
        }
        //post
        private async Task SignUpAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = RequestParser.ParsePostRequest<RequestUserName>(request.InputStream).UserName;
            var user = new User(userName, Type.User.ToString());
            var signUp = _messenger.SignUp(user);
            var answer = JsonConvert.SerializeObject(signUp, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            await ResponseWriter.WriteResponseAsync(answer, response.OutputStream);
        }
        //post
        private async Task CreateChatAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = RequestParser.ParsePostRequest<RequestChatName>(request.InputStream).ChatName;
            var chat = new Chat(chatName);
            var chats = _messenger.CreateChat(userName, chat);
            var answer = JsonConvert.SerializeObject(chats, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            await ResponseWriter.WriteResponseAsync(answer, response.OutputStream);
        }
        //get
        private async Task OpenChatAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = RequestParser.ParseGetRequestChatName(request.RawUrl).ChatName;
            var result = _messenger.OpenChat(userName, chatName);
            var answer = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            await ResponseWriter.WriteResponseAsync(answer, response.OutputStream);
        }
        //post
        private async Task DeleteChatAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = RequestParser.ParsePostRequest<RequestChatName>(request.InputStream).ChatName;
            var userChats = _messenger.DeleteChat(userName, chatName);
            var answer = JsonConvert.SerializeObject(userChats, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            await ResponseWriter.WriteResponseAsync(answer, response.OutputStream);
        }
        //get
        private async Task ExitChatAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = RequestParser.ParseGetRequestChatName(request.RawUrl).ChatName;
            var userChats = _messenger.ExitChat(userName, chatName);
            var answer = JsonConvert.SerializeObject(userChats, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            await ResponseWriter.WriteResponseAsync(answer, response.OutputStream);
        }
        //post
        private async Task AddMessageAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var parameters = RequestParser.ParsePostRequest<RequestChatNameAndMessageText>(request.InputStream);
            var chatName = parameters.ChatName;
            var textOfMessage = parameters.MessageText;
            var chatWithMessages = await _messenger.AddMessage(userName, chatName, textOfMessage);
            var answer = JsonConvert.SerializeObject(chatWithMessages, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            await ResponseWriter.WriteResponseAsync(answer, response.OutputStream);
        }
        //post
        private async Task AddUserToChatAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var parameters = RequestParser.ParsePostRequest<RequestUserNameAndChatName>(request.InputStream);
            var userName = parameters.UserName;
            var chatName = parameters.ChatName;
            var chatWithMesAndUsers = _messenger.AddUserToChat(userName, chatName);
            var answer = JsonConvert.SerializeObject(chatWithMesAndUsers, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            await ResponseWriter.WriteResponseAsync(answer, response.OutputStream);
        }
        //get
        private async Task DeleteMessageAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var parameters = RequestParser.ParseGetRequestChatNameAndMessageId(request.RawUrl);
            var chatName = parameters.ChatName;
            var messageId = parameters.MessageId;
            var messageDeleted = _messenger.DeleteMessage(userName, chatName, messageId);
            var answer = JsonConvert.SerializeObject(messageDeleted, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            await ResponseWriter.WriteResponseAsync(answer, response.OutputStream);
        }

        public async Task HandleAsync(HttpListenerRequest request, HttpListenerResponse response)
        {

            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("createChat")))
            {
                await CreateChatAsync(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestWithParamsTemplate("openChat")))
            {
                await OpenChatAsync(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("deleteChat")))
            {
                await DeleteChatAsync(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestWithParamsTemplate("exitChat")))
            {
                await ExitChatAsync(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("addMessage")))
            {
                await AddMessageAsync(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("addUserToChat")))
            {
                await AddUserToChatAsync(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestWithParamsTemplate("deleteMessage")))
            {
                await DeleteMessageAsync(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestWithParamsTemplate("signIn")))
            {
                await SignInAsync(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("signUp")))
            {
                await SignUpAsync(request, response);
            }
        }
    }
}