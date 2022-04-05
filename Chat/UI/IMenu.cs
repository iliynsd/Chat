using System.Collections.Generic;

namespace Chat.Utils
{
    public interface IMenu
    {
        public string SignIn();

        public void ShowChatWithLastMessage(Chat chat, string message, User user);

        public void ShowMainMenu();

        public void ShowAuthorizationPage();

        public void InvalidOperation();

        public void SignOut();

        public User SignUp();

        public void OpenChat(Chat chat, List<Message> messages, List<User> user);

        public void IncorrectUserName();

        public Chat CreateChat(List<User> users);

        public void ShowFitstChatCreate();

        public void SuccessSignUp();

        public void UserExists();

        public Message AddMessage(User user, Chat chat);

        public void ChatActions();

        public string InputTextOfMessage();

        public void SuccessfulDeleteMessage();

        public void NotDeleteMessage();

        public void SuccessfulDeleteChat();

        public void NotDeleteChat();

        public string GetChatName();

        public string GetUserName();
    }
}