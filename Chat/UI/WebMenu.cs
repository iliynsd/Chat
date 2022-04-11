using Chat.Utils;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Chat.UI
{
    public class Handler
    {
        public void Handle() { }
    }
    public class WebMenu : IMenu
    {
        public void AddMessage(string input)
        {
            throw new System.NotImplementedException();
        }

        public void AddUserToChat(string input)
        {
            throw new System.NotImplementedException();
        }

        public void CreateChat(string input)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteChat(string input)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteMessage(string input)
        {
            throw new System.NotImplementedException();
        }

        public void ExitChat(string input)
        {
            throw new System.NotImplementedException();
        }

        public void IncorrectUserName()
        {
            throw new System.NotImplementedException();
        }

        public void InvalidOperation()
        {
            throw new System.NotImplementedException();
        }

        public void OpenChat(string input)
        {
            throw new System.NotImplementedException();
        }

        public void ShowAuthorizationPage()
        {
            throw new System.NotImplementedException();
        }

        public void ShowChatPage(Chat chat, List<Message> messages, List<User> users)
        {
            throw new System.NotImplementedException();
        }

        public void ShowUserPage(List<Chat> userChats)
        {
            throw new System.NotImplementedException();
        }

        public void SignIn(string input)
        {
            throw new System.NotImplementedException();
        }

        public void SignUp(string input)
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:80/");
            listener.Start();

            while (true)
            {
                HttpListenerContext ctx = listener.GetContext();
                HttpListenerRequest request = ctx.Request;
                HttpListenerResponse response = ctx.Response;
                string text = string.Empty;
                if (request.RawUrl == "/signIn")
                {
                    text = File.ReadAllText(@"C:\Users\Administrator\source\repos\Chat\Chat\Templates\index.html");
                }
                else
                {
                    text = "hvukvk";
                }

                    using (var writer = new StreamWriter(response.OutputStream))
                    {
                        // stream.CopyTo(writer);
                        writer.WriteLine(text);
                    }
                }

            

            listener.Stop();
            listener.Close();
        }
    }
}
