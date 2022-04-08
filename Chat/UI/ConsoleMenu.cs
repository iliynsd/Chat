using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Chat.Utils
{
    public class ConsoleMenu : IMenu
    {

        private Messenger _messenger;
        private string Template(string cmd) => @$"^{cmd}\w*";

        public ConsoleMenu(Messenger messenger)
        {
            _messenger = messenger;
        }
        public void ShowAuthorizationPage()
        {
            Console.WriteLine("It's authorization page");
            Console.WriteLine("Enter signUp to pass registration or signIn if you have an account or exit to exit");
            var input = Console.ReadLine();
            if (Regex.IsMatch(input, Template("signIn")))
            {
                SignIn(input);
            }
            else if (Regex.IsMatch(input, Template("signUp")))
            {
                SignUp(input);
            }
            else
            {
                InvalidOperation();
                ShowAuthorizationPage();
            }
        }

        public void SignIn(string input)
        {
            var userName = input.Split(' ')[1];
            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("It's signIn menu");
                Console.WriteLine("Please enter your username to see your chats");

                userName = Console.ReadLine();
            }

            var userChats = _messenger.SignIn(userName);
            if (userChats is null)
            {
                ShowAuthorizationPage();
            }
            else
            {
                ShowUserPage(userChats);
            }
        }

        public void SignUp(string input)
        {
            var userName = input.Split(' ')[1];
            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("It's signUp menu");
                Console.WriteLine("Please enter your username to craete an account");

                userName = Console.ReadLine();
            }
            var user = new User(userName, Type.User.ToString(), true);

            var isSignedUp = _messenger.SignUp(user);
            if (isSignedUp)
            {
                Console.WriteLine("You successfully signed up");
            }
            else
            {
                Console.WriteLine("User with this nickName already exists");
            }

            ShowAuthorizationPage();
        }


        public void ShowUserPage(List<Chat> userChats)
        {
            Console.WriteLine("It's user page");
            userChats.ForEach(i => Console.WriteLine(i.Name));
            Console.WriteLine("ChatActions: Enter create-chat to create chat, open-chat to open chat or delete-chat to delete chat or exit-chat to exit chat");
            Console.WriteLine("Or enter sign-out to sign out from your account");
            var input = Console.ReadLine();
            if (Regex.IsMatch(input, Template("create-chat")))
            {
                CreateChat(input);
            }
            else if (Regex.IsMatch(input, Template("open-chat")))
            {
                OpenChat(input);
            }
            else if (Regex.IsMatch(input, Template("delete-chat")))
            {
                DeleteChat(input);
            }
            else if (Regex.IsMatch(input, Template("exit-chat")))
            {
                ExitChat(input);
            }
            else if (Regex.IsMatch(input, Template("sign-out")))
            {
                ShowAuthorizationPage();
            }
            else
            {
                InvalidOperation();
            }
        }

        public void InvalidOperation()
        {
            Console.WriteLine("Invalid operation, try one more time");
        }

        public void ShowChatPage(Chat chat, List<Message> messages, List<User> users)
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

            Console.WriteLine("Enter add-mes to add message or add-user to add user to chat or del-mes to delete message");

            var input = Console.ReadLine();
            if (Regex.IsMatch(input, Template("add-mes")))
            {
                AddMessage(input);
            }
            else if (Regex.IsMatch(input, Template("add-user")))
            {
                AddUserToChat(input);
            }
            else if (Regex.IsMatch(input, Template("del-mes")))
            {
                DeleteMessage(input);
            }
            else
            {
                InvalidOperation();
            }
        }

        public void OpenChat(string input)
        {

            var userName = input.Split(' ')[1];
            var chatName = input.Split(' ')[2];

            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Please enter your username");
                userName = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(chatName))
            {
                Console.WriteLine("Please enter chat name");
                chatName = Console.ReadLine();
            }

            var result = _messenger.OpenChat(userName, chatName);

            ShowChatPage(result.Item1, result.Item2, result.Item3);
        }


        public void IncorrectUserName()
        {
            Console.WriteLine("This user does not exist");
            ShowAuthorizationPage();
        }

        public void CreateChat(string input)
        {
            var userName = input.Split(' ')[1];
            var chatName = input.Split(' ')[2];

            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Please enter your username");
                userName = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(chatName))
            {
                Console.WriteLine("Please enter chat name");
                chatName = Console.ReadLine();
            }

            Console.WriteLine($"Chat with name - {chatName} is created");

            var chat = new Chat(chatName, true);
            var result = _messenger.CreateChat(userName, chat);
            ShowUserPage(result);
        }

        public void DeleteChat(string input)
        {
            var userName = input.Split(' ')[1];
            var chatName = input.Split(' ')[2];

            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Please enter your username");
                userName = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(chatName))
            {
                Console.WriteLine("Please enter chat name");
                chatName = Console.ReadLine();
            }

            var result = _messenger.DeleteChat(userName, chatName);
            ShowUserPage(result);
        }

        public void ExitChat(string input)
        {
            var userName = input.Split(' ')[1];
            var chatName = input.Split(' ')[2];

            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Please enter your username");
                userName = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(chatName))
            {
                Console.WriteLine("Please enter chat name");
                chatName = Console.ReadLine();
            }

            var result = _messenger.ExitChat(userName, chatName);
            ShowUserPage(result);
        }

        public void AddMessage(string input)
        {
            var userName = input.Split(' ')[1];
            var chatName = input.Split(' ')[2];
            var textOfMessage = input.Split(' ')[3];
            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Please enter your username");
                userName = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(chatName))
            {
                Console.WriteLine("Please enter chat name");
                chatName = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(textOfMessage))
            {
                Console.WriteLine("Enter text of message");
                textOfMessage = Console.ReadLine();
            }

           var result = _messenger.AddMessage(userName, chatName, textOfMessage);

        }

        public void DeleteMessage(string input)
        {
            var userName = input.Split(' ')[1];
            var chatName = input.Split(' ')[2];
            var textOfMessage = input.Split(' ')[3];
            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Please enter your username");
                userName = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(chatName))
            {
                Console.WriteLine("Please enter chat name");
                chatName = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(textOfMessage))
            {
                Console.WriteLine("Enter text of message");
                textOfMessage = Console.ReadLine();
            }

            _messenger.DeleteMessage(userName, chatName, textOfMessage);
        }

        public void AddUserToChat(string input)
        {
            var userName = input.Split(' ')[1];
            var chatName = input.Split(' ')[2];

            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Please enter your username");
                userName = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(chatName))
            {
                Console.WriteLine("Please enter chat name");
                chatName = Console.ReadLine();
            }

            _messenger.AddUserToChat(userName, chatName);
        }
    }
}