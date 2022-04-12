using Chat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Chat.UI
{
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

        }

        public void SignUp(string input)
        {
            throw new NotImplementedException();
        }
        public void Handle(Request request, HttpListenerResponse response)
        {
            string page = string.Empty;

            if (request.method == "/")
            {
                response.Redirect("http://localhost:80/signIn");
            }

            if (request.method == "/signIn")
            {
                page = File.ReadAllText(@"Templates\index.html");
            }
            if(request.method.Contains)

            if (request.method == "/signUp")
            {
                page = File.ReadAllText(@"Template\signUp.html");
            }

            using (var writer = new StreamWriter(response.OutputStream))
            {
                writer.WriteLine(page);
            }
        }
        public void Start()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:80/");
            listener.Start();

            while (true)
            {
                var context = listener.GetContext();
                var request = RequestParser.Parse(context.Request);
                var response = context.Response;
                Handle(request, response);
            }
        }
    }
}
