using System.Collections.Generic;

namespace Chat.Utils
{
    public interface IMenu
    {
        public void ShowAuthorizationPage();
        public void SignIn(string input);
        public void SignUp(string input);
        public void ShowUserPage(List<Chat> userChats);
        public void InvalidOperation();
        public void ShowChatPage(Chat chat, List<Message> messages, List<User> users);
        public void OpenChat(string input);
        public void IncorrectUserName();
        public void CreateChat(string input);
        public void DeleteChat(string input);
        public void ExitChat(string input);
        public void AddMessage(string input);
        public void DeleteMessage(string input);
        public void AddUserToChat(string input);
    }
}