using Chat.Bots;
using Chat.Models;
using Chat.Repositories;
using Chat.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public class Messenger : IHostedService
    {
        private Dictionary<string, MessengerCommand> _functional;
        private IMenu _menu;
        private IServiceProvider _serviceProvider;
        private delegate void MessengerCommand(string[] parametres);

        public Messenger(IMenu menu, [FromServices] IServiceProvider serviceProvider)
        {

            _functional = new Dictionary<string, MessengerCommand>()
            {
                {"signIn", SignIn},
                {"signUp", SignUp},
                {"sign-out", SignOut},
                {"create-chat", CreateChat},
                {"delete-chat", DeleteChat},
                {"open-chat", OpenChat},
                {"add-mes", AddMessage},
                {"del-mes", DeleteMessage},
                {"add-user", AddUserToChat},
                {"exit-chat", ExitChat},
                {"close-chat",  CloseChat}
            };

            _menu = menu;
            _serviceProvider = serviceProvider;
        }


        private void SignIn(string[] parametres)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var userName = FillUserName(parametres);

                if (users.IsUserExist(userName))
                {
                    var action = new ChatAction(ChatActions.UserSignIn(userName));
                    actions.Add(action);
                    actions.SaveToDb();
                    if (users.UserHasChats(userName))
                    {
                        var userChats = chats.GetAll().FindAll(i => i.Users.Contains(users.Get(userName)));

                        foreach (var chat in userChats)
                        {
                            if (messages.IsChatNotEmpty(chat))
                            {
                                var actionUserLeftChat = actions.Get($"User {userName} left chat - {chat.Name}");

                                var lastMessage = messages.GetChatMessages(chat).Last();
                                var authorOfLastMessage = users.GetAll().Find(i => lastMessage.UserId == i.Id);

                                if (actionUserLeftChat != null)
                                {
                                    lastMessage = messages.GetChatMessages(chat).FindAll(i => i.Time < actionUserLeftChat.Time).Last();
                                    authorOfLastMessage = users.GetAll().Find(i => lastMessage.UserId == i.Id);
                                }

                                _menu.ShowChatWithLastMessage(chat, lastMessage.Text, authorOfLastMessage);
                            }
                        }

                        _menu.ShowMainMenu();
                    }
                    else
                    {
                        _menu.ShowFitstChatCreate();
                    }
                }
                else
                {
                    _menu.IncorrectUserName();
                    _menu.ShowAuthorizationPage();
                }
            }
        }

        private void SignUp(string[] parametres)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var user = _menu.SignUp();

                while (true)
                {
                    if (users.IsUserExist(user.Name))
                    {
                        _menu.UserExists();
                        user = _menu.SignUp();
                    }
                    else
                    {
                        break;
                    }
                }

                users.Add(user);
                users.SaveToDb();

                var action = new ChatAction(ChatActions.UserSignUp(user.Name));
                actions.Add(action);
                actions.SaveToDb();

                _menu.SuccessSignUp();
            }
        }

        private void SignOut(string[] parametres)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var userName = FillUserName(parametres);
                actions.Add(new ChatAction(ChatActions.UserSignOut(userName)));
                actions.SaveToDb();
            }
            _menu.SignOut();
            _menu.ShowAuthorizationPage();
        }

        private void AddMessage(string[] parametres)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var botManager = scope.ServiceProvider.GetRequiredService<BotManager>();
                var clockBot = scope.ServiceProvider.GetRequiredService<ClockBot>();
                var botUploader = scope.ServiceProvider.GetRequiredService<BotUploader>();
                botManager.Subscribe(clockBot);
                botManager.Subscribe(botUploader);
                var userName = FillUserName(parametres);
                var chatName = FillChatName(parametres);

                var user = users.Get(userName);
                var message = _menu.AddMessage(user, chats.GetChat(chatName));

                messages.Add(message);
                var action = new ChatAction(ChatActions.UserAddMessage(userName, chatName, message.Text));

                actions.Add(action);
                actions.SaveToDb();
                messages.SaveToDb();
                botManager.Notify(action);
            }

            _menu.ChatActions();
        }

        private void OpenChat(string[] parametres)
        {
            var userName = FillUserName(parametres);
            var chatName = FillChatName(parametres);

            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var chat = chats.GetChat(chatName);
                messages.GetChatMessages(chat).ForEach(i => i.IsViewed = true);
                messages.SaveToDb();
                var chatUsers = users.GetAll().FindAll(i => chat.Users.Contains(i));

                var actionUserLeftChat = actions.Get($"User {userName} left chat - {chat.Name}");

                var actualMessages = messages.GetChatMessages(chat);

                if (actionUserLeftChat != null)
                {
                    actualMessages = messages.GetChatMessages(chat).FindAll(i => i.Time < actionUserLeftChat.Time);
                }

                _menu.OpenChat(chat, actualMessages, chatUsers);
                _menu.ChatActions();
            }
        }

        private void DeleteChat(string[] parametres)
        {
            var userName = FillUserName(parametres);
            var chatName = FillChatName(parametres);

            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var chat = chats.GetChat(chatName);
                if (chat != null)
                {
                    chats.Delete(chat);
                    var action = new ChatAction(ChatActions.UserDeleteChat(userName, chatName));

                    actions.Add(action);

                    _menu.SuccessfulDeleteChat();
                }
                else
                {
                    _menu.NotDeleteChat();
                }

                actions.SaveToDb();
                chats.SaveToDb();
            }

            _menu.ShowMainMenu();
        }

        private void DeleteMessage(string[] parametres)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var userName = FillUserName(parametres);
                var chatName = FillChatName(parametres);

                var textOfMessage = _menu.InputTextOfMessage();

                var chat = chats.GetChat(chatName);
                var message = messages.GetChatMessages(chat).Find(i => i.Text == textOfMessage);

                if (message != null && message.Time.AddDays(1) >= DateTime.Now)
                {
                    messages.Delete(message);
                    var action = new ChatAction(ChatActions.UserDeleteMessage(userName, chatName));
                    actions.Add(action);
                    _menu.SuccessfulDeleteMessage();
                }
                else
                {
                    _menu.NotDeleteMessage();
                }

                actions.SaveToDb();
                chats.SaveToDb();
                messages.SaveToDb();
            }

            _menu.ChatActions();
        }


        private void ExitChat(string[] parametres)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var userName = FillUserName(parametres);
                var chatName = FillChatName(parametres);

                var action = new ChatAction(ChatActions.UserLeftChat(userName, chatName));
                actions.Add(action);
                actions.SaveToDb();
            }

            _menu.ShowMainMenu();
        }

        private void CloseChat(string[] parametres)
        {
            _menu.ShowMainMenu();
        }

        private void CreateChat(string[] parametres)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var chat = _menu.CreateChat(users.GetAll());
                var action = new ChatAction(ChatActions.UserCreateChat(chat.Users.First().Name, chat.Name));
                actions.Add(action);
                chats.Add(chat);
                actions.SaveToDb();
                chats.SaveToDb();
                users.SaveToDb();
            }

            _menu.ShowMainMenu();
        }

        private void AddUserToChat(string[] parametres)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var userName = FillUserName(parametres);
                var chatName = FillChatName(parametres);

                var chat = chats.GetChat(chatName);
                var user = users.Get(userName);
                chat.Users.Add(user);
                chats.SaveToDb();
            }
            _menu.ChatActions();
        }
        public void Start()
        {
            _menu.ShowAuthorizationPage();
            var input = Console.ReadLine().Split(' ');
            var command = input.FirstOrDefault();
            var parameters = input.Skip(1).ToArray();

            while (command != "exit")
            {
                if (_functional.ContainsKey(command))
                {
                    _functional[command]?.Invoke(parameters);
                }
                else
                {
                    _menu.InvalidOperation();
                }

                input = Console.ReadLine().Split(' ');
                command = input.FirstOrDefault();
                parameters = input.Skip(1).ToArray();
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private string FillUserName(string[] parametres)
        {
            var userName = string.Empty;
            if (parametres.Count() > 0)
            {
                userName = parametres[0];
            }

            if (string.IsNullOrEmpty(userName))
            {
                userName = _menu.GetUserName();
            }

            return userName;
        }

        private string FillChatName(string[] parametres)
        {
            var chatName = string.Empty;
            if (parametres.Count() > 1)
            {
                chatName = parametres[1];
            }

            if (string.IsNullOrEmpty(chatName))
            {
                chatName = _menu.GetChatName();
            }

            return chatName;
        }
    }
}