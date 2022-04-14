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
            if(userName is null)
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
            if(chats is not null)
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
            if(chats is not null)
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
                var head = "<html lang='en' xmlns='http://www.w3.org/1999/xhtml'><head><meta charset='UTF-8'><meta content='width=device-width, initial-scale=1.0'><style>ul.hr { margin: 0; padding: 4px; } ul.hr li { display: inline; margin - right: 5px; border: 1px solid #000; padding: 3px; }</style></head>";
                var body = $"<body><h1>{result.Item1.Name}</h1><ul class='hr'><li><button onclick='deleteChat();'>Delete chat</button></li><li><button onclick='redirectToUserPage();'>Close chat</button></li><li><button>Delete message</button></li><li><button>Add user to chat</button></li><li><button onclick='exitChat();'>Exit chat</button></li></ul>";
                foreach (var message in result.Item2)
                {
                    body += $"<div style='padding-left:90px;' style='padding-top:10px'>{result.Item3.Find(i => i.Id == message.UserId).Name} - {message.Text}</div>";
                }

                var end = "<form style='padding-left:90px;' style='padding-top:10px' action='/openChat/" + chatName + "/addMessage' method='post'><input type='text' name='textOfMessage' required=''><button>Send</button></form><script>function redirectToUserPage() { window.location='http://localhost:80/userPage'; } </script><script>function deleteChat() { window.location='http://localhost:80/deleteChat/" + chatName + "'; } </script><script>function exitChat() { window.location='http://localhost:80/exitChat/" + chatName + "'; } </script></body></html>";
                
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

        private void AddUserToChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            var parameters = RequestParser.ParseParams(request.InputStream);

            if (parameters is null)
            {
                item = File.ReadAllText(@"Templates\addUserToChat.html");
                ResponseWriter.WriteResponse(item, response.OutputStream);
            }
            else
            {
                var userName = parameters[0].ToString();
                var chatName = parameters[1].ToString();
                var result = _messenger.AddUserToChat(userName, chatName);
                item = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                ResponseWriter.WriteResponse(item, response.OutputStream);
            }
        }

        private void DeleteMessage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = request.Cookies.First(i => i.Name == "chatName").Value;
            var parameters = request.RawUrl.Split('/');
            if (parameters.Length == 3)
            {
                var textOfMessage = parameters[2];
                var result = _messenger.DeleteMessage(userName, chatName, textOfMessage);
                item = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }

            ResponseWriter.WriteResponse(item, response.OutputStream);
        }

        private void ShowUserPage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var userName = request.Cookies.FirstOrDefault(i => i.Name == "userName")?.Value;
            var chatNames = _messenger.SignIn(userName)?.Select(i => i.Name);

            var head = "<html lang='en' xmlns='http://www.w3.org/1999/xhtml'><head><meta charset='UTF-8'><meta content='width=device-width, initial-scale=1.0'><style>ul.hr { margin: 0; padding: 4px; } ul.hr li { display: inline; margin - right: 5px; border: 1px solid #000; padding: 3px; }</style></head><body><h1>User page</h1><ul class='hr'><li><a href='createChat'>Create chat</a></li><li><a href=''>Add user to chat</a></li><li><a href='http://localhost:80/'>Sign out</a></li></ul>";
            var body = "<ul>";
            foreach(var chatName in chatNames)
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
            if(request.RawUrl == "/makeChat")
            {
                CreateChat(request, response);
            }
            if (request.RawUrl.Contains("/openChat") && request.RawUrl.Count(i => i == '/')==2)
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
            if (request.RawUrl == "/addUserToChat")
            {
                AddUserToChat(request, response);
            }
            if (request.RawUrl.Contains("/deleteMessage"))
            {
                DeleteMessage(request, response);
            }
            if(request.RawUrl == "/userPage")
            {
                ShowUserPage(request, response);
            }
            if(request.RawUrl == "/createSession")
            {
                SignIn(request, response);
            }
            if(request.RawUrl == "/createAccount")
            {
                SignUp(request, response);
            }
        }
    }
}
