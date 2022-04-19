using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Chat.Web
{
    public partial class RequestHandler
    {
        private const string Messages = "<!--Messages-->";
        private const string ChatName = "<!--ChatName-->";
        private const string UserName = "<!--UserName-->";
        private const string Chats = "<!--Chats-->";

        partial void Authorize(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.FirstOrDefault(i => i.Name == "userName")?.Value;
            if (userName is null)
            {
                response.Redirect(_options.Protocol + _options.Host + _options.Port + "signIn");
            }
            else
            {
                response.Redirect(_options.Protocol + _options.Host + _options.Port + "userPage");
            }

            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        partial void ShowSignInPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = File.ReadAllText(@"Templates\index.html");
            ResponseWriter.WriteResponse(file, response.OutputStream);
        }

        partial void SignIn(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = RequestParser.ParseParams(request.InputStream)[0].ToString();
            var chats = _messenger.SignIn(userName)?.Select(i => i.Name);
            if (chats is not null)
            {
                var cookie = new Cookie("userName", userName);
                response.AppendCookie(cookie);
            }

            response.Redirect(_options.Protocol + _options.Host + _options.Port);
            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        partial void ShowSignUpPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = File.ReadAllText(@"Templates\signUp.html");
            ResponseWriter.WriteResponse(file, response.OutputStream);
        }

        partial void SignUp(HttpListenerRequest request, HttpListenerResponse response)
        {

            var parameters = RequestParser.ParseParams(request.InputStream);

            var user = new User(parameters[0].ToString(), Type.User.ToString());
            var signUp = _messenger.SignUp(user);

            if (!signUp)
            {
                response.Redirect(_options.Protocol + _options.Host + _options.Port + "signUp");
            }
            else
            {
                response.Redirect(_options.Protocol + _options.Host + _options.Port + "signIn");
            }

            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        partial void ShowCreateChatPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = File.ReadAllText(@"Templates\createChat.html");
            ResponseWriter.WriteResponse(file, response.OutputStream);
        }

        partial void CreateChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var parameters = RequestParser.ParseParams(request.InputStream);
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = parameters[0].ToString();
            var chat = new Chat(chatName);
            var chats = _messenger.CreateChat(userName, chat);
            if (chats is not null)
            {
                response.Redirect(_options.Protocol + _options.Host + _options.Port + "userPage");
            }
            else
            {
                response.Redirect(_options.Protocol + _options.Host + _options.Port + "createChat");
            }

            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        partial void OpenChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var responsePage = "Not found";
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var parameters = request.RawUrl.Split('/');

            if (parameters.Length == 3)
            {
                var chatName = parameters[2];
                var result = _messenger.OpenChat(userName, chatName);
                responsePage = File.ReadAllText(@"Templates\chatPage.html");
                var messages = string.Empty;
                responsePage = responsePage.Replace("'<!--NumMes-->'", result.messages.Count().ToString());

                foreach (var message in result.messages)
                {
                    messages += $"{result.users.Find(i => i.Id == message.UserId).Name} - {message.Text}";
                    messages += ';';
                }

                responsePage =  responsePage.Replace(Messages, messages);
                responsePage = responsePage.Replace(ChatName, chatName);
                response.Cookies.Add(new Cookie("chatName", chatName));
            }

            ResponseWriter.WriteResponse(responsePage, response.OutputStream);
        }

        partial void DeleteChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var parameters = request.RawUrl.Split('/');

            if (parameters.Length == 3)
            {
                var chatName = parameters[2];
                _messenger.DeleteChat(userName, chatName);
                response.Redirect(_options.Protocol + _options.Host + _options.Port + "userPage");
            }

            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        partial void ExitChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var parameters = request.RawUrl.Split('/');

            if (parameters.Length == 3)
            {
                var chatName = parameters[2];
                _messenger.ExitChat(userName, chatName);
                response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
            }

            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        partial void AddMessage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = request.Cookies.First(i => i.Name == "chatName").Value;

            var textOfMessage = RequestParser.ParseParams(request.InputStream)[0].ToString();
            _messenger.AddMessage(userName, chatName, textOfMessage);
            response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        partial void ShowAddUserToChatPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = File.ReadAllText(@"Templates\addUserToChat.html");
            ResponseWriter.WriteResponse(file, response.OutputStream);
        }

        partial void AddUserToChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var parameters = RequestParser.ParseParams(request.InputStream);

            if (parameters is null)
            {
                ResponseWriter.WriteResponse("Not found", response.OutputStream);
            }
            else
            {
                var userName = parameters[0].ToString();
                var chatName = request.Cookies.FirstOrDefault(i => i.Name == "chatName").Value;
                _messenger.AddUserToChat(userName, chatName);
                response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
                ResponseWriter.WriteResponse("", response.OutputStream);
            }
        }

        partial void DeleteMessage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = request.Cookies.First(i => i.Name == "chatName").Value;

            var parameters = RequestParser.ParseParams(request.RawUrl);
            if (parameters.Count > 0)
            {
                var messageId = Convert.ToInt32(parameters[0]);
                _messenger.DeleteMessage(userName, chatName, messageId);
            }

            response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        partial void ShowUserPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.FirstOrDefault(i => i.Name == "userName")?.Value;
            var chatNames = _messenger.SignIn(userName)?.Select(i => i.Name);
            var responsePage = File.ReadAllText(@"Templates\userPage.html");
            var chats = string.Empty;
            responsePage = responsePage.Replace("'<!--NumChats-->'", chatNames.Count().ToString());

            foreach(var chat in chatNames)
            {
                chats += chat;
                chats += ';';
            }

            responsePage = responsePage.Replace(Chats, chats);
            responsePage = responsePage.Replace(UserName, userName);
            ResponseWriter.WriteResponse(responsePage, response.OutputStream);
        }
    }
}