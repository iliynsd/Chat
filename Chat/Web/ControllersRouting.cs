using System.Linq;
using System.Net;

namespace Chat.Web
{
   public partial class RequestHandler : IHandler
    {
        private Messenger _messenger;

        public RequestHandler(Messenger messenger)
        {
            _messenger = messenger;
        }

        partial void Authorize(HttpListenerRequest request, HttpListenerResponse response);
        partial void ShowSignInPage(HttpListenerRequest request, HttpListenerResponse response);
        partial void ShowSignUpPage(HttpListenerRequest request, HttpListenerResponse response);
        partial void SignUp(HttpListenerRequest request, HttpListenerResponse response);
        partial void SignIn(HttpListenerRequest request, HttpListenerResponse response);
        partial void ShowUserPage(HttpListenerRequest request, HttpListenerResponse response);
        partial void ShowCreateChatPage(HttpListenerRequest request, HttpListenerResponse response);
        partial void CreateChat(HttpListenerRequest request, HttpListenerResponse response);
        partial void OpenChat(HttpListenerRequest request, HttpListenerResponse response);
        partial void DeleteChat(HttpListenerRequest request, HttpListenerResponse response);
        partial void ExitChat(HttpListenerRequest request, HttpListenerResponse response);
        partial void AddMessage(HttpListenerRequest request, HttpListenerResponse response);
        partial void ShowAddUserToChatPage(HttpListenerRequest request, HttpListenerResponse response);
        partial void AddUserToChat(HttpListenerRequest request, HttpListenerResponse response);
        partial void DeleteMessage(HttpListenerRequest request, HttpListenerResponse response);

        public void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
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