using System;
using System.Collections.Generic;
using System.Linq;
using Chat.Repositories;
using Chat.UI;

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

        public void ShowChats(List<Chat> chats, List<string> lastMessages, List<User> users)
        {
            Console.WriteLine("Your chats:");
            if (chats.Count > 0)
            {
                for (int i = 0; i < chats.Count; i++)
                {
                    UIConsole.ShowChatWithLastMessage(chats[i], lastMessages[i], users[i]);
                }
            }
            else
            {
                Console.WriteLine("You have not got any chat yet");
            }
        }

        public void ShowMainMenu()
        {
            Console.WriteLine("It's a main menu");
            Console.WriteLine("Enter create-chat to create chat, open-chat to open chat or delete-chat to delete chat");
        }

        public void ShowAuthorizationPage()
        {
             Console.WriteLine("It's authorization page");
             Console.WriteLine("Enter signUp to pass registration or signIn if you have an account");
        }


        public void InvalidOperation()
        {
            Console.WriteLine("Invalid operation, try one more time");
        }

        public void SignOut()
        {
            Console.WriteLine("You are sign out");
        }

        public User SignUp(IUserRepository users)
        {
            Console.WriteLine("Let's create an account");
            
            Console.WriteLine("Enter your userName");
                var username = Console.ReadLine();

                var userId = users.GetAll().Last().Id++;
            return new User()
            {
                Id = userId,
                Name = username,
                ChatIds = new List<int>(),
                IsActive = true,
                Type = Type.User.ToString()
            };
        }

        public void OpenChat(Chat chat, List<Message> messages, List<User> users)
        {
            Console.WriteLine($"-----Chat {chat.Name}-----");
            foreach (var message in messages)
            {
                Console.WriteLine(users.Find(i => i.Id == message.UserId).Name);
                Console.WriteLine(message.Text + " " +  message.Time);
            }
        }

        public string GetChatNameToOpen()
        {
            Console.WriteLine("Enter name of chat to open");
            return Console.ReadLine();
        }

        public void IncorrectUserName()
        {
            Console.WriteLine("This user does not exist");
            Console.WriteLine("Pass registration");
        }

        public Chat CreateChat(List<User> users, List<Chat> chats)
        {
            Console.WriteLine("Enter your username:");
            var username = Console.ReadLine();
            Console.WriteLine("Enter chat name:");
            var chatName = Console.ReadLine();
            Console.WriteLine("Enter user who you join the chat");
            var companionUsername = Console.ReadLine();
            int chatId = 0;
            if(chats.Count() > 0)
            {
                chatId = chats.Last().Id++;
            }
            
            var userIds = new List<int>();
            userIds.Add(users.Find(i => i.Name == username).Id);
            userIds.Add(users.Find(i => i.Name == companionUsername).Id);
            users.Find(i => i.Name == username).ChatIds.Add(chatId);
            users.Find(i => i.Name == companionUsername).ChatIds.Add(chatId);
            Console.WriteLine($"Chat with name - {chatName} is created");
            Console.WriteLine("Enter open-chat to open, delete-chat to delete");
            return new Chat() 
            {
                Id = chatId,
                IsActive = true,
                Name = chatName,
                UserIds = userIds
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
            Console.WriteLine("Enter signIn to log into your account");
        }

        public void ShowChat(Chat chat, User user, Message message)
        {
            Console.WriteLine($"Chat - {chat.Name}");
            Console.WriteLine($"Last message - {message.Text}");
            Console.WriteLine($"Author of last message - {user.Type} {user.Name}");
        }

        public Message AddMessage(IMessageRepository messages, IChatRepository chats, IUserRepository users)
        {
            Console.WriteLine("Enter your userName");
            var user = users.Get(Console.ReadLine());
            Console.WriteLine("Enter chat name");
            var chat = chats.GetAll().Find(i => i.Name == Console.ReadLine());
            Console.WriteLine("Enter text of message");
            var messageText = Console.ReadLine();
            var chatMessages = messages.GetAll().GroupBy(i => i.ChatId == chat.Id).SelectMany(group => group);
            int id = 0; ;
            if(chatMessages.Count()>0)
            {
               id = chatMessages.Last().Id++;
            }
            
            return new Message()
            {
                Id = id,
                Text = messageText,
                IsActive = true,
                IsViewed = false,
                Time = DateTime.Now,
                ChatId = chat.Id,
                UserId = user.Id
            };
        }
    }
}