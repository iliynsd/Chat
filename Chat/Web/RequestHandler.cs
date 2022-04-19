using System.IO;
using System.Linq;
using System.Net;

namespace Chat.Web
{
    public partial class RequestHandler
    {
        
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
                var head = "<html lang='en' xmlns='http://www.w3.org/1999/xhtml'><head><meta charset='UTF-8'><meta content='width=device-width, initial-scale=1.0'><style>ul.hr { margin: 0; padding: 4px; } ul.hr li { display: inline; margin - right: 5px; border: 1px solid #000; padding: 3px; } div.textField { margin: 0; padding: 0; font-size:20px; padding-left:90px; padding-top:16px; } a.block-1 {font-size:22px;}</style></head>";
                var body = $"<body><h1>{result.chat.Name}</h1><ul class='hr'><li><button onclick='deleteChat();'>Delete chat</button></li><li><button onclick='redirectToUserPage();'>Close chat</button></li><li><button onclick='addUserToChat();'>Add user to chat</button></li><li><button onclick='exitChat();'>Exit chat</button></li></ul><div style='padding-top:30px;'></div>";

                foreach (var message in result.messages)
                {
                    body += $"<div class='textField'><a>{result.users.Find(i => i.Id == message.UserId).Name} - {message.Text}</a><button onclick='deleteMessage{message.Id}();'> Delete message </button></div>";
                    body += "<script>function deleteMessage" + message.Id + "() { window.location='" +_options.Protocol + _options.Host + _options.Port + "openChat/" + chatName + "/deleteMessage?textOfMessage=" + message.Text + "'; }</script>";
                }

                var end = "<form style='padding-left:90px;' style='padding-top:10px' action='/openChat/" + chatName + "/addMessage' method='post'><input type='text' name='textOfMessage' required=''><button>Send</button></form><script>function redirectToUserPage() { window.location='" + _options.Protocol + _options.Host + _options.Port + "userPage'; } </script><script>function deleteChat() { window.location='" +_options.Protocol + _options.Host +_options.Port + "deleteChat/" + chatName + "'; } </script><script>function exitChat() { window.location='" + _options.Protocol + _options.Host + _options.Port + "exitChat/" + chatName + "'; } </script><script>function addUserToChat() { window.location='" + _options.Protocol + _options.Host + _options.Port + "openChat/" + chatName + "/addUserToChat.html'; }</script></body></html>";

                responsePage = head + body + end;
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
            if (parameters.Count >0)
            {
                var textOfMessage = parameters[0].ToString();
                _messenger.DeleteMessage(userName, chatName, textOfMessage);
            }

            response.Redirect(_options.Protocol + _options.Host + _options.Port + $"openChat/{chatName}");
            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        partial void ShowUserPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.FirstOrDefault(i => i.Name == "userName")?.Value;
            var chatNames = _messenger.SignIn(userName)?.Select(i => i.Name);

            var head = "<html lang='en' xmlns='http://www.w3.org/1999/xhtml'><head><meta charset='UTF-8'><meta content='width=device-width, initial-scale=1.0'><style>ul.hr { margin: 0; padding: 4px; } ul.hr li { display: inline; margin - right: 5px; border: 1px solid #000; padding: 3px; }</style></head><body><h1>User page</h1><ul class='hr'><li><a href='createChat'>Create chat</a></li><li>" + $"<a href='{_options.Protocol + _options.Host + _options.Port}'>Sign out</a></li></ul>";
            var body = "<ul>";
            foreach (var chatName in chatNames)
            {
                body += $"<li><a href='openChat/{chatName}'>{chatName}</a></li>";
            }
            var end = "</ul></body></html>";
            var file = head + body + end;
            ResponseWriter.WriteResponse(file, response.OutputStream);
        }
    }
}