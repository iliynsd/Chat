using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Utils
{
    public class ConsoleMenu : IMenu
    {
        public string SignIn()
        {
            Console.WriteLine("It's signIn menu");
            Console.WriteLine("Please enter your username to see your chats");
            return Console.ReadLine();
        }

        public void ShowChatWithLastMessage(Chat chat, string message, User user)
        {
            Console.WriteLine(chat.Name);
            Console.WriteLine("By " + user.Type + " " + user.Name + " " + message);
        }

        public void ShowMainMenu()
        {
            Console.WriteLine("It's a main menu");
            Console.WriteLine("ChatActions: Enter create-chat to create chat, open-chat to open chat or delete-chat to delete chat or exit-chat to exit chat");
            Console.WriteLine("Or enter sign-out to sign out from your account");
        }

        public void ShowAuthorizationPage()
        {
            Console.WriteLine("It's authorization page");
            Console.WriteLine("Enter signUp to pass registration or signIn if you have an account or exit to exit");
        }


        public void InvalidOperation()
        {
            Console.WriteLine("Invalid operation, try one more time");
        }

        public void SignOut()
        {
            Console.WriteLine("You are sign out");
        }

        public User SignUp()
        {
            Console.WriteLine("Let's create an account");

            var username = GetUserName();

            return new User()
            {
                Name = username,
                IsActive = true,
                Type = Type.User.ToString()
            };
        }

        public void OpenChat(Chat chat, List<Message> messages, List<User> users)
        {
            Console.WriteLine($"-----Chat {chat.Name}-----");
            if (messages.Count > 0)
            {

                foreach (var message in messages)
                {
                    var user = users.Find(i => i.Id == message.UserId);

                    if (user != null)
                    {
                        Console.WriteLine(user.Name);
                    }


                    Console.WriteLine(message.Text + " " + message.Time);
                }
            }
            else
            {
                Console.WriteLine("Haven't got any messages in this chat yet");
            }
        }


        public void IncorrectUserName()
        {
            Console.WriteLine("This user does not exist");
            Console.WriteLine("Pass registration");
        }

        public Chat CreateChat(List<User> users)
        {
            var username = GetUserName();

            var chatName = GetChatName();

            Console.WriteLine("Enter user who you join the chat");
            var companionUsername = Console.ReadLine();

            var chatUsers = new List<User>();
            chatUsers.Add(users.FirstOrDefault(i => i.Name == username));
            chatUsers.Add(users.FirstOrDefault(i => i.Name == companionUsername));
            Console.WriteLine($"Chat with name - {chatName} is created");
            Console.WriteLine("Enter open-chat to open, delete-chat to delete");
            return new Chat()
            {
                IsActive = true,
                Name = chatName,
                Users = chatUsers
            };
        }

        public void ShowFitstChatCreate()
        {
            Console.WriteLine("You have not got any chats, let's create");
            Console.WriteLine("Enter create-chat");
        }

        public void SuccessSignUp()
        {
            Console.WriteLine("You have successfully signed up");
            ShowAuthorizationPage();
        }

        public Message AddMessage(User user, Chat chat)
        {
            Console.WriteLine("Enter text of message");
            var messageText = Console.ReadLine();

            return new Message()
            {
                Text = messageText,
                IsActive = true,
                IsViewed = false,
                Time = DateTime.Now,
                ChatId = chat.Id,
                UserId = user.Id
            };
        }

        public void ChatActions()
        {
            Console.WriteLine("Enter add-mes to add message or add-user to add user to chat or del-mes to delete message or close-chat to close chat");
        }


        public string InputTextOfMessage()
        {
            Console.WriteLine("Enter text of message to delete");
            return Console.ReadLine();
        }

        public void SuccessfulDeleteChat()
        {
            Console.WriteLine("Chat was deleted");
        }

        public void NotDeleteChat()
        {
            Console.WriteLine("Chat was not deleted");
        }

        public string GetChatName()
        {
            Console.WriteLine("Enter chat name");
            return Console.ReadLine();
        }

        public string GetUserName()
        {
            Console.WriteLine("Enter user name");
            return Console.ReadLine();
        }

        public void UserExists()
        {
            Console.WriteLine("User with this nickname already exists");
        }

        public void SuccessfulDeleteMessage()
        {
            Console.WriteLine("Message was deleted");
        }

        public void NotDeleteMessage()
        {
            Console.WriteLine("Message wasn't deleted");
        }
    }
}