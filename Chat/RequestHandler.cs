using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Chat
{
    public class RequestHandler : IHandler
    { 
        private Messenger _messenger;
        private string Template(string method) => "/openChat";
        public RequestHandler(Messenger messenger)
        {
            _messenger = messenger;
        }

        private void Authorize(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.FirstOrDefault(i => i.Name == "userName")?.Value;
            if (userName is null)
            {
                response.Redirect("http://localhost:80/signIn");
            }
            else
            {
                response.Redirect("http://localhost:80/userPage");
            }

            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        private void ShowSignInPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = File.ReadAllText(@"Templates\index.html");
            ResponseWriter.WriteResponse(file, response.OutputStream);
        }

        private void SignIn(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = RequestParser.ParseParams(request.InputStream)[0].ToString();
            var chats = _messenger.SignIn(userName)?.Select(i => i.Name);
            if (chats is not null)
            {
                var cookie = new Cookie("userName", userName);
                response.AppendCookie(cookie);
            }


            response.Redirect("http://localhost:80/");
            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        private void ShowSignUpPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = File.ReadAllText(@"Templates\signUp.html");
            ResponseWriter.WriteResponse(file, response.OutputStream);
        }

        private void SignUp(HttpListenerRequest request, HttpListenerResponse response)
        {

            var parameters = RequestParser.ParseParams(request.InputStream);

            var user = new User(parameters[0].ToString(), Type.User.ToString());
            var signUp = _messenger.SignUp(user);

            if (!signUp)
            {
                response.Redirect("http://localhost:80/signUp");
            }
            else
            {
                response.Redirect("http://localhost:80/signIn");
            }

            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        private void ShowCreateChatPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = File.ReadAllText(@"Templates\createChat.html");
            ResponseWriter.WriteResponse(file, response.OutputStream);
        }

        private void CreateChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var parameters = RequestParser.ParseParams(request.InputStream);
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = parameters[0].ToString();
            var chat = new Chat(chatName);
            var chats = _messenger.CreateChat(userName, chat);
            if (chats is not null)
            {
                response.Redirect("http://localhost:80/userPage");
            }
            else
            {
                response.Redirect("http://localhost:80/createChat");
            }

            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        private void OpenChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();

            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var parameters = request.RawUrl.Split('/');

            if (parameters.Length == 3)
            {
                var chatName = parameters[2];

                var result = _messenger.OpenChat(userName, chatName);
                var head = "<html lang='en' xmlns='http://www.w3.org/1999/xhtml'><head><meta charset='UTF-8'><meta content='width=device-width, initial-scale=1.0'><style>ul.hr { margin: 0; padding: 4px; } ul.hr li { display: inline; margin - right: 5px; border: 1px solid #000; padding: 3px; } div.textField { margin: 0; padding: 0; font-size:20px; padding-left:90px; padding-top:16px; } a.block-1 {font-size:22px;}</style></head>";
                var body = $"<body><h1>{result.Item1.Name}</h1><ul class='hr'><li><button onclick='deleteChat();'>Delete chat</button></li><li><button onclick='redirectToUserPage();'>Close chat</button></li><li><button onclick='addUserToChat();'>Add user to chat</button></li><li><button onclick='exitChat();'>Exit chat</button></li></ul><div style='padding-top:30px;'></div><form action='/openChat/" + chatName + "/deleteMessage' method='post'><button id='del-mes'>Delete message</button>";
                
                foreach (var message in result.Item2)
                {
                    body += $"<div class='textField'><a>{result.Item3.Find(i => i.Id == message.UserId).Name} - </a><a onclick='selectText();' class='block-1'>{message.Text}</a></div>";
                }

                var end = "<script>function selectText() { document.querySelector('.block-2').value = document.querySelector('.block-1').textContent; }) }</script><input class='block-2' name='textOfMessage' type='text' style='display:none;' ></form><form style='padding-left:90px;' style='padding-top:10px' action='/openChat/" + chatName + "/addMessage' method='post'><input type='text' name='textOfMessage' required=''><button>Send</button></form><script>function redirectToUserPage() { window.location='http://localhost:80/userPage'; } </script><script>function deleteChat() { window.location='http://localhost:80/deleteChat/" + chatName + "'; } </script><script>function exitChat() { window.location='http://localhost:80/exitChat/" + chatName + "'; } </script><script>function addUserToChat() { window.location='http://localhost:80/openChat/" + chatName + "/addUserToChat.html'; }</script></body></html>";

                item = head + body + end;
                response.Cookies.Add(new Cookie("chatName", chatName));
            }

            ResponseWriter.WriteResponse(item, response.OutputStream);
        }

        private void DeleteChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var parameters = request.RawUrl.Split('/');

            if (parameters.Length == 3)
            {
                var chatName = parameters[2];
                var result = _messenger.DeleteChat(userName, chatName);
                response.Redirect("http://localhost:80/userPage");
            }

            ResponseWriter.WriteResponse(item, response.OutputStream);
        }


        private void ExitChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var parameters = request.RawUrl.Split('/');

            if (parameters.Length == 3)
            {
                var chatName = parameters[2];
                var result = _messenger.ExitChat(userName, chatName);
                response.Redirect($"http://localhost:80/openChat/{chatName}");
            }

            ResponseWriter.WriteResponse(item, response.OutputStream);
        }

        private void AddMessage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = request.Cookies.First(i => i.Name == "chatName").Value;

            var textOfMessage = RequestParser.ParseParams(request.InputStream)[0].ToString();
            var result = _messenger.AddMessage(userName, chatName, textOfMessage);
            response.Redirect($"http://localhost:80/openChat/{chatName}");
            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        public void ShowAddUserToChatPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var file = File.ReadAllText(@"Templates\addUserToChat.html");
            ResponseWriter.WriteResponse(file, response.OutputStream);
        }

        private void AddUserToChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            var parameters = RequestParser.ParseParams(request.InputStream);

            if (parameters is null)
            {
                ResponseWriter.WriteResponse("Not found", response.OutputStream);
            }
            else
            {
                var userName = parameters[0].ToString();
                var chatName = request.Cookies.FirstOrDefault(i => i.Name == "chatName").Value;
                var result = _messenger.AddUserToChat(userName, chatName);
                response.Redirect($"http://localhost:80/openChat/{chatName}");
                ResponseWriter.WriteResponse(item, response.OutputStream);
            }
        }

        private void DeleteMessage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = request.Cookies.First(i => i.Name == "chatName").Value;

            var rez = new StreamReader(request.InputStream).ReadLine();
            Console.WriteLine(rez);
            var parameters = RequestParser.ParseParams(request.InputStream);
            if (parameters is not null)
            {
                var textOfMessage = parameters[0].ToString();
                _messenger.DeleteMessage(userName, chatName, textOfMessage);
            }
            response.Redirect($"http://localhost:80/openChat/{chatName}");
            ResponseWriter.WriteResponse("", response.OutputStream);
        }

        private void ShowUserPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.FirstOrDefault(i => i.Name == "userName")?.Value;
            var chatNames = _messenger.SignIn(userName)?.Select(i => i.Name);

            var head = "<html lang='en' xmlns='http://www.w3.org/1999/xhtml'><head><meta charset='UTF-8'><meta content='width=device-width, initial-scale=1.0'><style>ul.hr { margin: 0; padding: 4px; } ul.hr li { display: inline; margin - right: 5px; border: 1px solid #000; padding: 3px; }</style></head><body><h1>User page</h1><ul class='hr'><li><a href='createChat'>Create chat</a></li><li><a href='http://localhost:80/'>Sign out</a></li></ul>";
            var body = "<ul>";
            foreach (var chatName in chatNames)
            {
                body += $"<li><a href='openChat/{chatName}'>{chatName}</a></li>";
            }
            var end = "</ul></body></html>";
            var file = head + body + end;
            ResponseWriter.WriteResponse(file, response.OutputStream);
        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            object item = new object();

            Console.WriteLine(request.RawUrl);

            if (request.RawUrl == "/")
            {
                Authorize(request, response);
            }
            if (request.RawUrl == "/signIn")
            {
                ShowSignInPage(request, response);
            }
            if (request.RawUrl == "/signUp")
            {
                ShowSignUpPage(request, response);
            }
            if (request.RawUrl == "/createChat")
            {
                ShowCreateChatPage(request, response);
            }
            if (request.RawUrl == "/makeChat")
            {
                CreateChat(request, response);
            }
            if (request.RawUrl.Contains("/openChat") && request.RawUrl.Count(i => i == '/') == 2)
            {
                OpenChat(request, response);
            }
            if (request.RawUrl.Contains("/deleteChat"))
            {
                DeleteChat(request, response);
            }
            if (request.RawUrl.Contains("/exitChat"))
            {
                ExitChat(request, response);
            }
            if (request.RawUrl.Contains("/addMessage"))
            {
                AddMessage(request, response);
            }
            if (request.RawUrl.Contains("/addUserToChat.html"))
            {
                ShowAddUserToChatPage(request, response);
            }

            if (request.RawUrl.Contains("/addUserToChat"))
            {
                AddUserToChat(request, response);
            }
            if (request.RawUrl.Contains("/deleteMessage"))
            {
                DeleteMessage(request, response);
            }
            if (request.RawUrl == "/userPage")
            {
                ShowUserPage(request, response);
            }
            if (request.RawUrl == "/createSession")
            {
                SignIn(request, response);
            }
            if (request.RawUrl == "/createAccount")
            {
                SignUp(request, response);
            }
        }
    }
}
