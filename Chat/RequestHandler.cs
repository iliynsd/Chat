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

        public RequestHandler(Messenger messenger)
        {
            _messenger = messenger;
        }

        private void SignIn(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            var parameters = RequestParser.ParseParams(request.InputStream);

            if (parameters is null)
            {
                item = File.ReadAllText(@"Templates\index.html");

                ResponseWriter.WriteResponse(item, response.OutputStream);
            }
            else
            {
                var userName = parameters[0].ToString();
                var chats = _messenger.SignIn(userName)?.Select(i => i.Name);
                if (chats is null)
                {
                    response.StatusCode = 400;
                    item = File.ReadAllText(@"Templates\index.html");
                }
                else
                {
                    response.Redirect("http://localhost:80/userPage");
                    item = JsonConvert.SerializeObject(chats, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    response.Cookies.Add(new Cookie("userName", userName));
                }

                ResponseWriter.WriteResponse(item, response.OutputStream);
            }
        }

        private void SignUp(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            var parameters = RequestParser.ParseParams(request.InputStream);

            if (parameters is null)
            {
                item = File.ReadAllText(@"Templates\signUp.html");
                ResponseWriter.WriteResponse(item, response.OutputStream);
            }
            else
            {
                var user = new User(parameters[0].ToString(), Type.User.ToString());
                var signUp = _messenger.SignUp(user);

                if (!signUp)
                {
                    response.StatusCode = 400;
                    item = File.ReadAllText(@"Templates\signUp.html");
                }
                else
                {
                    item = File.ReadAllText(@"Templates\index.html");
                }

                ResponseWriter.WriteResponse(item, response.OutputStream);
            }
        }

        private void CreateChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            var parameters = RequestParser.ParseParams(request.InputStream);
            if (parameters is null)
            {
                item = File.ReadAllText(@"Templates\createChat.html");
                ResponseWriter.WriteResponse(item, response.OutputStream);
            }
            else
            {
                var userName = request.Cookies.First(i => i.Name == "userName").Value;
                var chatName = parameters[0].ToString();
                var chat = new Chat(chatName);
                var chats = _messenger.CreateChat(userName, chat);
                item = JsonConvert.SerializeObject(chats, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                ResponseWriter.WriteResponse(item, response.OutputStream);
            }
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
                item = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
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
                item = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }

            ResponseWriter.WriteResponse(item, response.OutputStream);
        }

        private void CloseChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            response.Redirect("http://localhost:80/userPage");
            ResponseWriter.WriteResponse(item, response.OutputStream);
        }

        private void SignOut(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            response.Redirect("http://localhost:80/signIn");
            var cook = request.Cookies.First(i => i.Name == "userName");
            Console.WriteLine(cook.Value);
            Console.WriteLine(request.Cookies.Count);
            request.Cookies.Remove(cook);
            Console.WriteLine(request.Cookies.Count);

            ResponseWriter.WriteResponse(item, response.OutputStream);
        }

        private void ExitChat(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = request.Cookies.First(i => i.Name == "chatName").Value;

            var result = _messenger.ExitChat(userName, chatName);
            item = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            ResponseWriter.WriteResponse(item, response.OutputStream);
        }



        private void AddMessage(HttpListenerRequest request, HttpListenerResponse response)
        {
            var item = new object();
            var userName = request.Cookies.First(i => i.Name == "userName").Value;
            var chatName = request.Cookies.First(i => i.Name == "chatName").Value;
            var parameters = request.RawUrl.Split('/');
            Console.WriteLine(parameters.Length);
            if (parameters.Length == 5)
            {
                var textOfMessage = parameters[4];
                var result = _messenger.AddMessage(userName, chatName, textOfMessage);
                Console.WriteLine("---------------");

                item = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                Console.WriteLine(item);
            }

            ResponseWriter.WriteResponse(item, response.OutputStream);
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

        }

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            object item = new object();

            if (request.RawUrl == "/")
            {
                response.Redirect("http://localhost:80/signIn");
                ResponseWriter.WriteResponse(item, response.OutputStream);
            }
            if (request.RawUrl == "/signIn")
            {
                SignIn(request, response);
            }
            if (request.RawUrl == "/signUp")
            {
                SignUp(request, response);
            }
            if (request.RawUrl == "/createChat")
            {
                CreateChat(request, response);
            }

            if (request.RawUrl.Contains("/openChat"))
            {
                OpenChat(request, response);
            }
            if (request.RawUrl == "/closeChat")
            {
                CloseChat(request, response);
            }
            if (request.RawUrl.Contains("/deleteChat"))
            {
                DeleteChat(request, response);
            }
            if (request.RawUrl == "/exitChat")
            {
                ExitChat(request, response);
            }
            if (request.RawUrl == "/signOut")
            {
                SignOut(request, response);
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
        }
    }
}
