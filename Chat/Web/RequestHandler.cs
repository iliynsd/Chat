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
        private const string NumMes = "<!--NumMes-->";
        private const string MessagesIds = "<!--MesIds-->";

        async partial void Authorize(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.AsParallel().FirstOrDefault(i => i.Name == "userName")?.Value;
            if (userName is null)
            {
                response.Redirect(_options.Protocol + _options.Host + _options.Port + "signIn");
            }
            else
            {
                response.Redirect(_options.Protocol + _options.Host + _options.Port + "userPage");
            }

           await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async partial void ShowSignInPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = await File.ReadAllTextAsync(@"Templates\index.html");
            await ResponseWriter.WriteResponseAsync(file, response.OutputStream);
        }

        async partial void SignIn(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = RequestParser.ParseParams(request.InputStream)[0].ToString();
            var chats = _messenger.SignIn(userName)?.AsParallel().Select(i => i.Name);
            if (chats is not null)
            {
                var cookie = new Cookie("userName", userName);
                response.AppendCookie(cookie);
            }

            response.Redirect(_options.Protocol + _options.Host + _options.Port);
            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async partial void ShowSignUpPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = await File.ReadAllTextAsync(@"Templates\signUp.html");
            await ResponseWriter.WriteResponseAsync(file, response.OutputStream);
        }

        async partial void SignUp(HttpListenerRequest request, HttpListenerResponse response)
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
            
            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async partial void ShowCreateChatPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = await File.ReadAllTextAsync(@"Templates\createChat.html");
            await ResponseWriter.WriteResponseAsync(file, response.OutputStream);
        }

        async partial void CreateChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var parameters = RequestParser.ParseParams(request.InputStream);
            var userName = request.Cookies.AsParallel().First(i => i.Name == "userName").Value;
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

            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async partial void OpenChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var responsePage = "Not found";
            var userName = request.Cookies.AsParallel().First(i => i.Name == "userName").Value;
            var parameters = request.RawUrl.Split('/');

            if (parameters.Length == 3)
            {
                var chatName = parameters[2];
                var result = _messenger.OpenChat(userName, chatName);
                responsePage = await File.ReadAllTextAsync(@"Templates\chatPage.html");
                var messages = string.Empty;
                responsePage = responsePage.Replace(NumMes, result.messages.Count().ToString());
                
                foreach (var message in result.messages)
                {
                    messages += $"{result.users.Find(i => i.Id == message.UserId).Name} - {message.Text}";
                    messages += ';';
                }

                var messagesIds = string.Empty;
                foreach (var id in result.messages.AsParallel().Select(i => i.Id))
                {
                    messagesIds += id;
                    messagesIds += ';';
                }

                responsePage = responsePage.Replace(MessagesIds, messagesIds);
                responsePage = responsePage.Replace(Messages, messages);
                responsePage = responsePage.Replace(ChatName, chatName);
                response.Cookies.Add(new Cookie("chatName", chatName));
            }

            await ResponseWriter.WriteResponseAsync(responsePage, response.OutputStream);
        }

        async partial void DeleteChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.AsParallel().First(i => i.Name == "userName").Value;
            var parameters = request.RawUrl.Split('/');

            if (parameters.Length == 3)
            {
                var chatName = parameters[2];
                _messenger.DeleteChat(userName, chatName);
                response.Redirect(_options.Protocol + _options.Host + _options.Port + "userPage");
            }

            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async partial void ExitChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.AsParallel().First(i => i.Name == "userName").Value;
            var parameters = request.RawUrl.Split('/');

            if (parameters.Length == 3)
            {
                var chatName = parameters[2];
                _messenger.ExitChat(userName, chatName);
                response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
            }

            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async partial void AddMessage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.AsParallel().First(i => i.Name == "userName").Value;
            var chatName = request.Cookies.AsParallel().First(i => i.Name == "chatName").Value;

            var textOfMessage = RequestParser.ParseParams(request.InputStream)[0].ToString();
            _messenger.AddMessage(userName, chatName, textOfMessage);
            response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async partial void ShowAddUserToChatPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = await File.ReadAllTextAsync(@"Templates\addUserToChat.html");
            await ResponseWriter.WriteResponseAsync(file, response.OutputStream);
        }

        async partial void AddUserToChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var parameters = RequestParser.ParseParams(request.InputStream);

            if (parameters is null)
            {
                await ResponseWriter.WriteResponseAsync("Not found", response.OutputStream);
            }
            else
            {
                var userName = parameters[0].ToString();
                var chatName = request.Cookies.AsParallel().FirstOrDefault(i => i.Name == "chatName").Value;
                _messenger.AddUserToChat(userName, chatName);
                response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
                await ResponseWriter.WriteResponseAsync("", response.OutputStream);
            }
        }

        async partial void DeleteMessageAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.AsParallel().First(i => i.Name == "userName").Value;
            var chatName = request.Cookies.AsParallel().First(i => i.Name == "chatName").Value;

            var parameters = RequestParser.ParseParams(request.RawUrl);
            if (parameters.Count > 0)
            {
                var messageId = Convert.ToInt32(parameters[0]);
                _messenger.DeleteMessage(userName, chatName, messageId);
            }

            response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async partial void ShowUserPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.AsParallel().FirstOrDefault(i => i.Name == "userName")?.Value;
            var chatNames = _messenger.SignIn(userName)?.AsParallel().Select(i => i.Name);
            var responsePage = await File.ReadAllTextAsync(@"Templates\userPage.html");
            var chats = string.Empty;
            responsePage = responsePage.Replace("'<!--NumChats-->'", chatNames.Count().ToString());

            foreach (var chat in chatNames)
            {
                chats += chat;
                chats += ';';
            }

            responsePage = responsePage.Replace(Chats, chats);
            responsePage = responsePage.Replace(UserName, userName);
            await ResponseWriter.WriteResponseAsync(responsePage, response.OutputStream);
        }
    }
}