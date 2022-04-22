using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Web
{
    public  class RequestHandler : IHandler
    {
        private const string Messages = "<!--Messages-->";
        private const string ChatName = "<!--ChatName-->";
        private const string UserName = "<!--UserName-->";
        private const string Chats = "<!--Chats-->";
        private const string NumMes = "<!--NumMes-->";
        private const string MessagesIds = "<!--MesIds-->";
        private Messenger _messenger;
        private AppOptions.Options _options;
        private string Root = "/";
        private string RootRequestWithParamsTemplate(string method) => @$"/{method}\w*";
        private string RootRequestTemplate(string method) => @$"^/{method}$";
        private string OpenChatPageRequest => @"/openChat/\w*$";
        private string ChatPageRequests(string method) => @$"/openChat/\w*/{method}\w*";

        public RequestHandler(Messenger messenger, IOptions<AppOptions.Options> options)
        {
            _messenger = messenger;
            _options = options.Value;
        }
        private async void Authorize(HttpListenerRequest request, HttpListenerResponse response)
        {
            await Task.Factory.StartNew(() =>
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

                Thread.Sleep(600);
            });
            

           await ResponseWriter.WriteResponseAsync("", response.OutputStream);
            }
        private async Task ShowSignInPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = await File.ReadAllTextAsync(@"Templates\index.html");
            await ResponseWriter.WriteResponseAsync(file, response.OutputStream);
        }

        private async Task SignIn(HttpListenerRequest request, HttpListenerResponse response)
        {
            await Task.Factory.StartNew(() =>
            {
                var userName = RequestParser.ParseParams(request.InputStream)[0].ToString();
                var chats = _messenger.SignIn(userName)?.AsParallel().Select(i => i.Name);
                if (chats is not null)
                {
                    var cookie = new Cookie("userName", userName);
                    response.AppendCookie(cookie);
                }

                response.Redirect(_options.Protocol + _options.Host + _options.Port);
            });
            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        private async Task ShowSignUpPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = await File.ReadAllTextAsync(@"Templates\signUp.html");
            await ResponseWriter.WriteResponseAsync(file, response.OutputStream);
        }

        private async Task SignUp(HttpListenerRequest request, HttpListenerResponse response)
        {
            await Task.Factory.StartNew(() =>
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
            });

            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        private async Task ShowCreateChatPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = await File.ReadAllTextAsync(@"Templates\createChat.html");
            await ResponseWriter.WriteResponseAsync(file, response.OutputStream);
        }

        private async Task CreateChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            await Task.Factory.StartNew(() =>
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
            });

            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        private async Task OpenChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var responsePage = "Not found";
            await Task.Factory.StartNew(() =>
            {
                
                var userName = request.Cookies.AsParallel().First(i => i.Name == "userName").Value;
                var parameters = request.RawUrl.Split('/');

                if (parameters.Length == 3)
                {
                    var chatName = parameters[2];
                    var result = _messenger.OpenChat(userName, chatName);
                    responsePage =  File.ReadAllText(@"Templates\chatPage.html");
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
            });

            await ResponseWriter.WriteResponseAsync(responsePage, response.OutputStream);
        }

       private async Task DeleteChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            await Task.Factory.StartNew(() =>
            {
                var userName = request.Cookies.AsParallel().First(i => i.Name == "userName").Value;
                var parameters = request.RawUrl.Split('/');

                if (parameters.Length == 3)
                {
                    var chatName = parameters[2];
                    _messenger.DeleteChat(userName, chatName);
                    response.Redirect(_options.Protocol + _options.Host + _options.Port + "userPage");
                }
            });

            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async Task ExitChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            await Task.Factory.StartNew(() =>
            {
                var userName = request.Cookies.AsParallel().First(i => i.Name == "userName").Value;
                var parameters = request.RawUrl.Split('/');

                if (parameters.Length == 3)
                {
                    var chatName = parameters[2];
                    _messenger.ExitChat(userName, chatName);
                    response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
                }
            });

            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async Task AddMessage(HttpListenerRequest request, HttpListenerResponse response)
        {
            await Task.Factory.StartNew(() =>
            {
                var userName = request.Cookies.AsParallel().First(i => i.Name == "userName").Value;
                var chatName = request.Cookies.AsParallel().First(i => i.Name == "chatName").Value;

                var textOfMessage = RequestParser.ParseParams(request.InputStream)[0].ToString();
                _messenger.AddMessage(userName, chatName, textOfMessage);
                response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
            });

            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async Task ShowAddUserToChatPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = await File.ReadAllTextAsync(@"Templates\addUserToChat.html");
            await ResponseWriter.WriteResponseAsync(file, response.OutputStream);
        }

        async Task AddUserToChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var resp = "Not found";
            await Task.Factory.StartNew(() =>
            {
                var parameters = RequestParser.ParseParams(request.InputStream);

                
                if(parameters is not null)
                { 
                    var userName = parameters[0].ToString();
                    var chatName = request.Cookies.AsParallel().FirstOrDefault(i => i.Name == "chatName").Value;
                    _messenger.AddUserToChat(userName, chatName);
                    response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
                    resp = "";
                }
            });

            await ResponseWriter.WriteResponseAsync(resp, response.OutputStream);
        }

        async void DeleteMessageAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
           await Task.Factory.StartNew(() =>
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
            });
            await ResponseWriter.WriteResponseAsync("", response.OutputStream);
        }

        async void ShowUserPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var responsePage = await File.ReadAllTextAsync(@"Templates\userPage.html");
            await Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                var userName = request.Cookies.AsParallel().FirstOrDefault(i => i.Name == "userName")?.Value;
                var chatNames = _messenger.SignIn(userName)?.AsParallel().Select(i => i.Name);

                var chats = string.Empty;
                responsePage = responsePage.Replace("'<!--NumChats-->'", chatNames.Count().ToString());

                foreach (var chat in chatNames)
                {
                    chats += chat;
                    chats += ';';
                }

                responsePage = responsePage.Replace(Chats, chats);
                responsePage = responsePage.Replace(UserName, userName);
            });
            await ResponseWriter.WriteResponseAsync(responsePage, response.OutputStream);
        }

        public async Task HandleAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            Console.WriteLine(request.RawUrl);

            if (request.RawUrl == Root)
            {
                 Authorize(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("signIn")))
            {
                 ShowSignInPage(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("signUp")))
            {
                 ShowSignUpPage(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("createChat")))
            {
                 ShowCreateChatPage(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("makeChat")))
            {
                CreateChat(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, OpenChatPageRequest))
            {
                OpenChat(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestWithParamsTemplate("deleteChat")))
            {
                DeleteChat(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestWithParamsTemplate("exitChat")))
            {
                ExitChat(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, ChatPageRequests("addMessage")))
            {
                AddMessage(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, ChatPageRequests("addUserToChat.html")))
            {
                ShowAddUserToChatPage(request, response);
            }

            if (Regex.IsMatch(request.RawUrl, ChatPageRequests("addUserToChat")))
            {
                AddUserToChat(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, ChatPageRequests("deleteMessage")))
            {
                DeleteMessageAsync(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("userPage")))
            {
                ShowUserPage(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("createSession")))
            {
                SignIn(request, response);
            }
            if (Regex.IsMatch(request.RawUrl, RootRequestTemplate("createAccount")))
            {
                SignUp(request, response);
            }
        }
    }
}