using System.Collections.Generic;
using Chat.Repositories;

namespace Chat.Utils
{
    public interface IMenu
    {
        public string SignIn();
        
        public void ShowChat(Chat chat, User user, Message message);

        public void ShowMainMenu();

        public void ShowAuthorizationPage();
        
        public void InvalidOperation();

        public void SignOut();

        public User SignUp(IUserRepository users);

        public void OpenChat(Chat chat, List<Message> messages, List<User> user);

        public string GetChatNameToOpen();

        public void IncorrectUserName();

        public Chat CreateChat(List<User> users, List<Chat> chats);

        public void ShowFitstChatCreate();

        public void SuccessSignUp();

        public Message AddMessage(IMessageRepository messages, IChatRepository chats, IUserRepository users);
    }
}