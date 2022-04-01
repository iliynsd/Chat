using Chat.Repositories;
using System;
using System.Collections.Generic;

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

        public User SignUp(List<User> users)
        {
            Console.WriteLine("Let's create an account");
            Console.WriteLine("Enter your userName");
            var username = Console.ReadLine();

            if (users.FindAll(i => i.Name == username).Count > 0)
            {
                Console.WriteLine("User with this nickname already exists");
                SignUp(users);
            }

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

        public Chat CreateChat(IUserRepository users, IChatRepository chats)
        {
            Console.WriteLine("Enter your username:");
            var username = Console.ReadLine();
            if (users.GetAll().FindAll(i => i.Name == username).Count < 1)
            {
                Console.WriteLine("Have not got this user");
                CreateChat(users, chats);
            }
            Console.WriteLine("Enter chat name:");
            var chatName = Console.ReadLine();
            if (chats.GetAll().FindAll(i => i.Name == chatName).Count > 0)
            {
                Console.WriteLine("Have got chat with this name");
                CreateChat(users, chats);
            }
            Console.WriteLine("Enter user who you join the chat");
            var companionUsername = Console.ReadLine();
            if (users.GetAll().FindAll(i => i.Name == companionUsername).Count < 1)
            {
                Console.WriteLine("Have not got this user");
                CreateChat(users, chats);
            }



            var chatUsers = new List<User>();
            chatUsers.Add(users.Get(username));
            chatUsers.Add(users.Get(companionUsername));
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
            Console.WriteLine("Enter signIn to log into your account");
        }

        public Message AddMessage(IMessageRepository messages, IChatRepository chats, IUserRepository users)
        {
            Console.WriteLine("Enter your userName");
            var user = users.Get(Console.ReadLine());
            Console.WriteLine("Enter chat name");
            var chat = chats.GetAll().Find(i => i.Name == Console.ReadLine());
            Console.WriteLine("Enter text of message");
            var messageText = Console.ReadLine();
            var chatMessages = messages.GetChatMessages(chat);

            Console.WriteLine("Message was add");

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
            Console.WriteLine("Enter add-mess to add message or del-mes to delete message or enter read-mes to read messages in chat");
        }

        public void ReadMessage(IChatRepository chats, IMessageRepository messages)
        {
            Console.WriteLine("Enter chat name in which read messages");
            var chat = chats.GetAll().Find(i => i.Name == Console.ReadLine());
            messages.GetAll().FindAll(i => i.ChatId == chat.Id).ForEach(i => i.IsViewed = true);
            Console.WriteLine("Messages were read");
        }

        public void DeleteMessage(IChatRepository chats, IMessageRepository messages)
        {
            Console.WriteLine("Enter chat name in which delete message");
            var chat = chats.GetAll().Find(i => i.Name == Console.ReadLine());
            Console.WriteLine("Enter text of message to delete");
            var message = messages.GetAll().FindAll(i => i.ChatId == chat.Id).Find(i => i.Text == Console.ReadLine());
            if (message != null)
            {
                if (message.Time.AddDays(1) >= DateTime.Now)
                {
                    messages.Delete(message);
                    Console.WriteLine("Message was deleted");
                }
                else
                {
                    Console.WriteLine("Wasn't deleted, too old");
                }
            }
            else
            {
                Console.WriteLine("Haven't got this message");
            }
        }

        public void DeleteChat(IChatRepository chats)
        {
            Console.WriteLine("Enter chat name to delete");
            chats.Delete(chats.GetAll().Find(i => i.Name == Console.ReadLine()));
            Console.WriteLine("Chat was deleted");
        }
    }
}