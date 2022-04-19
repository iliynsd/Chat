using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Chat.Web
{
    public partial class RequestHandler : IHandler
    {
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
                DeleteMessage(request, response);
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