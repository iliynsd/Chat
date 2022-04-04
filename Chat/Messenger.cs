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
        private Dictionary<string, Action> _functional;
        private IMenu _menu;
        private IServiceProvider _serviceProvider;

        public Messenger(IMenu menu, [FromServices] IServiceProvider serviceProvider)
        {

            _functional = new Dictionary<string, Action>()
            {
                {"signIn", SignIn},
                {"try", Try},
                {"signUp", SignUp},
                {"sign-out", SignOut},
                {"create-chat", CreateChat},
                {"delete-chat", DeleteChat},
                {"open-chat", OpenChat},
                {"add-mes", AddMessage},
                {"del-mes", DeleteMessage},
                {"add-user", AddUserToChat},
                {"exit-chat", ExitChat},
                {"bot", BotInvoke},
                {"close-chat",  CloseChat}
            };

            _menu = menu;
            _serviceProvider = serviceProvider;
        }


        private void Try()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var user1 = new User() { Type = "Bot", Name = "tryuser1", IsActive = true };
                var user2 = new User() { Type = "Bot", Name = "tryuser2", IsActive = true };
                var user3 = new User() { Type = "Bot", Name = "tryuser3", IsActive = true };
                var user4 = new User() { Type = "Bot", Name = "tryuser4", IsActive = true };
                users.Add(user1);
                users.Add(user2);
                users.Add(user3);
                users.Add(user4);
                users.SaveToDb();

                chats.Add(new Chat() { Name = "chat1", IsActive = true, Users = new List<User>() { user1, user2, user3 } });
                chats.Add(new Chat() { Name = "chat2", IsActive = true, Users = new List<User>() { user3, user2, user4 } });
                chats.Add(new Chat() { Name = "chat3", IsActive = true, Users = new List<User>() { user1, user2, user4 } });

                chats.SaveToDb();

                messages.Add(new Message() { Text = "textuser1", ChatId = 1, UserId = 1, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser2", ChatId = 1, UserId = 2, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser3", ChatId = 1, UserId = 3, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser3", ChatId = 2, UserId = 3, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser2", ChatId = 2, UserId = 2, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser4", ChatId = 2, UserId = 4, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser1", ChatId = 3, UserId = 1, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser2", ChatId = 3, UserId = 2, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser4", ChatId = 3, UserId = 4, IsActive = true, IsViewed = false, Time = DateTime.Now });

                messages.SaveToDb();

            }

        }


        private void SignIn()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var userName = _menu.SignIn();

                if (users.IsUserExist(userName))
                {
                    var action = CreateChatAction(ChatActions.UserSignIn(userName));
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

        private void SignUp()
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


                var action = CreateChatAction(ChatActions.UserSignUp(user.Name));
                actions.Add(action);

                actions.SaveToDb();
                _menu.SuccessSignUp();
            }
        }

        private void SignOut()
        {
            _menu.SignOut();
            _menu.ShowAuthorizationPage();
        }

        private void AddMessage()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                //var bots = null;
                //TODO create botmanager

                var userName = _menu.GetUserName();
                var chatName = _menu.GetChatName();
                var user = users.Get(userName);
                var message = _menu.AddMessage(user, chats.GetChat(chatName));

                messages.Add(message);
                actions.Add(CreateChatAction(ChatActions.UserAddMessage(userName)));
                actions.SaveToDb();
                messages.SaveToDb();
                chats.SaveToDb();
                users.SaveToDb();
            }
            _menu.ChatActions();
        }

        private void OpenChat()
        {
            var userName = _menu.GetUserName();
            var chatName = _menu.GetChatNameToOpen();
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

        private void DeleteChat()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var chatName = _menu.GetChatName();
                var userName = _menu.GetUserName();

                var chat = chats.GetChat(chatName);
                if (chat != null)
                {
                    chats.Delete(chat);
                    var action = CreateChatAction(ChatActions.UserDeleteChat(userName, chatName));

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

        private void DeleteMessage()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var chatAndTextOfMessage = _menu.InputChatAndTextOfMessage();
                var userName = _menu.GetUserName();

                var chat = chats.GetChat(chatAndTextOfMessage.Item1);
                var message = messages.GetChatMessages(chat).Find(i => i.Text == chatAndTextOfMessage.Item2);

                if (message != null && message.Time.AddDays(1) >= DateTime.Now)
                {
                    messages.Delete(message);
                    var action = CreateChatAction(ChatActions.UserDeleteMessage(userName));
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


        private void ExitChat()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var userName = _menu.GetUserName();
                var chatName = _menu.GetChatName();
                var action = CreateChatAction(ChatActions.UserLeftChat(userName, chatName));
                actions.Add(action);
                actions.SaveToDb();
            }

            _menu.ShowMainMenu();
        }

        private void CloseChat()
        {
            _menu.ShowMainMenu();
        }

        private void BotInvoke() { }

        private void CreateChat()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var chat = _menu.CreateChat(users.GetAll());
                var action = CreateChatAction(ChatActions.UserCreateChat(chat.Users.First().Name, chat.Name));
                actions.Add(action);
                chats.Add(chat);
                actions.SaveToDb();
                chats.SaveToDb();
                users.SaveToDb();
            }

            _menu.ShowMainMenu();
        }

        private void AddUserToChat()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var chat = chats.GetChat(_menu.GetChatName());
                var user = users.Get(_menu.GetUserName());
                chat.Users.Add(user);
                chats.SaveToDb();
            }
            _menu.ChatActions();
        }
        public void Start()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
            }

            _menu.ShowAuthorizationPage();
            var cmd = Console.ReadLine();


            while (cmd != "exit")
            {
                if (_functional.ContainsKey(cmd))
                {
                    _functional[cmd]?.Invoke();
                }
                else
                {
                    _menu.InvalidOperation();
                }

                cmd = Console.ReadLine();
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

        private ChatAction CreateChatAction(string actionText)
        {
            var action = new ChatAction();
            action.Time = DateTime.Now;
            action.ActionText = actionText;
            return action;
        }
    }
}