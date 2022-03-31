using Chat.Repositories;
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

        public User SignUp(List<User> users);

        public void OpenChat(Chat chat, List<Message> messages, List<User> user);

        public string GetChatNameToOpen();

        public void IncorrectUserName();

        public Chat CreateChat(IUserRepository users, IChatRepository chats);

        public void ShowFitstChatCreate();

        public void SuccessSignUp();

        public Message AddMessage(IMessageRepository messages, IChatRepository chats, IUserRepository users);

        public void ChatActions();

        public void DeleteMessage(IChatRepository chats, IMessageRepository messages);

        public void DeleteChat(IChatRepository chats);
    }
}