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

        public void Start()
        {
            ShowAuthorizationPage();
        }
        public void ShowAuthorizationPage()
        {
            Console.WriteLine("It's authorization page");
            Console.WriteLine("Enter signUp to pass registration or signIn if you have an account");
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
            var userName = GetUserName(input);

            var userChats = _messenger.SignIn(userName);
            if (userChats is null)
            {
                Console.WriteLine("Have not got this user");
                ShowAuthorizationPage();
            }
            else
            {
                ShowUserPage(userChats);
            }
        }

        public void SignUp(string input)
        {

            var userName = GetUserName(input);
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

            Console.WriteLine("Enter add-mes to add message or add-user to add user to chat or del-mes to delete message or close-chat to close chat");

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
            else if (Regex.IsMatch(input, Template("close-chat")))
            {
                CloseChat(input);
            }
            else
            {
                InvalidOperation();
                ShowChatPage(chat, messages, users);
            }
        }

        public void OpenChat(string input)
        {
            var userName = GetUserName(input);
            var chatName = GetChatName(input);
            var result = _messenger.OpenChat(userName, chatName);
            ShowChatPage(result.Item1, result.Item2, result.Item3);
        }

        public void CloseChat(string input)
        {
            var userName = GetUserName(input);
            var chats = _messenger.SignIn(userName);
            ShowUserPage(chats);
        }
        public void IncorrectUserName()
        {
            Console.WriteLine("This user does not exist");
            ShowAuthorizationPage();
        }

        public void CreateChat(string input)
        {
            var userName = GetUserName(input);
            var chatName = GetChatName(input);

            Console.WriteLine($"Chat with name - {chatName} is created");

            var chat = new Chat(chatName, true);
            var result = _messenger.CreateChat(userName, chat);
            ShowUserPage(result);
        }

        public void DeleteChat(string input)
        {
            var userName = GetUserName(input);
            var chatName = GetChatName(input);
            var result = _messenger.DeleteChat(userName, chatName);
            ShowUserPage(result);
        }

        public void ExitChat(string input)
        {
            var userName = GetUserName(input);
            var chatName = GetChatName(input);
            var result = _messenger.ExitChat(userName, chatName);
            ShowUserPage(result);
        }

        public void AddMessage(string input)
        {
            var userName = GetUserName(input);
            var chatName = GetChatName(input);
            var textOfMessage = GetTextOfMessage(input);
            var result = _messenger.AddMessage(userName, chatName, textOfMessage);
            ShowChatPage(result.Item1, result.Item2, result.Item3);
        }

        public void DeleteMessage(string input)
        {
            var userName = GetUserName(input);
            var chatName = GetChatName(input);
            var textOfMessage = GetTextOfMessage(input);
            var result = _messenger.DeleteMessage(userName, chatName, textOfMessage);
            ShowChatPage(result.Item1, result.Item2, result.Item3);
        }

        public void AddUserToChat(string input)
        {
            var userName = GetUserName(input);
            var chatName = GetChatName(input);
            var result = _messenger.AddUserToChat(userName, chatName);
            ShowChatPage(result.Item1, result.Item2, result.Item3);
        }

        public string GetUserName(string input)
        {
            var parameters = input.Split(' ');
            var userName = string.Empty;

            if (parameters.Length > 1)
            {
                userName = parameters[1];
            }
            else
            {
                Console.WriteLine("Enter your username");
                userName = Console.ReadLine();
            }

            return userName;
        }

        public string GetChatName(string input)
        {
            var parameters = input.Split(' ');
            var chatName = string.Empty;

            if (parameters.Length > 2)
            {
                chatName = parameters[2];
            }
            else
            {
                Console.WriteLine("Enter name of chat");
                chatName = Console.ReadLine();
            }

            return chatName;
        }

        public string GetTextOfMessage(string input)
        {
            var parameters = input.Split(' ');
            var textOfMessage = string.Empty;

            if (parameters.Length > 3)
            {
                textOfMessage = parameters[3];
            }
            else
            {
                Console.WriteLine("Enter text of message");
                textOfMessage = Console.ReadLine();
            }

            return textOfMessage;
        }
    }
}