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
            if (messages.Count > 0)
            {
               
                foreach (var message in messages)
                {
                    var user = users.Find(i => i.Id == message.UserId);
                    if(user !=null)
                     {
                            Console.WriteLine(user.Name);
                    }
                     
                    Console.WriteLine(message.Text + " " + message.Time);
                }
            }
            else
            {
                Console.WriteLine("Haven't got any messages in this chat yet");
                Console.WriteLine("Enter add-mes to add message");
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
        Console.WriteLine("Enter chat name:");
        var chatName = Console.ReadLine();
        Console.WriteLine("Enter user who you join the chat");
        var companionUsername = Console.ReadLine();
        int chatId = 0;
        if (chats.GetAll().Count() > 0)
        {
            chatId = chats.GetAll().Last().Id++;
        }

        var userIds = new List<int>();
        userIds.Add(users.Get(username).Id);
        userIds.Add(users.Get(companionUsername).Id);
        users.Get(username).ChatIds.Add(chatId);
        users.Get(companionUsername).ChatIds.Add(chatId);
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



    public Message AddMessage(IMessageRepository messages, IChatRepository chats, IUserRepository users)
    {
        Console.WriteLine("Enter your userName");
        var user = users.Get(Console.ReadLine());
        Console.WriteLine("Enter chat name");
        var chat = chats.GetAll().Find(i => i.Name == Console.ReadLine());
        Console.WriteLine("Enter text of message");
        var messageText = Console.ReadLine();
        var chatMessages = messages.GetChatMessages(chat);
        int id = 0; ;
        if (chatMessages.Count() > 0)
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

    public string ChatActions()
    {
        Console.WriteLine("Enter add-mess to add message or del-mes to delete message or enter read-mes to read messages in chat");
        return Console.ReadLine();
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