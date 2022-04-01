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
                {"signOut", SignOut},
                {"create-chat", CreateChat},
                {"delete-chat", DeleteChat},
                {"open-chat", OpenChat},
                {"add-mes", AddMessage},
                {"del-mes", DeleteMessage},
                {"exit-chat", ExitChat},
                {"bot", BotInvoke}
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
                var user1 = new User() { Type = "Bot", Name = "tryuser5", IsActive = true };
                var user2 = new User() { Type = "Bot", Name = "tryuser6", IsActive = true };
                var user3 = new User() { Type = "Bot", Name = "tryuser7", IsActive = true };
                var user4 = new User() { Type = "Bot", Name = "tryuser8", IsActive = true };
                users.Add(user1);
                users.Add(user2);
                users.Add(user3);
                users.Add(user4);
                users.SaveToDb();

                chats.Add(new Chat() { Name = "chat4", IsActive = true, Users = new List<User>() { user1, user2, user3 } });
                chats.Add(new Chat() { Name = "chat5", IsActive = true, Users = new List<User>() { user3, user2, user4 } });
                chats.Add(new Chat() { Name = "chat6", IsActive = true, Users = new List<User>() { user1, user2, user4 } });

                chats.SaveToDb();

                messages.Add(new Message() { Text = "textuser1", ChatId = 3, UserId = 1, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser2", ChatId = 3, UserId = 2, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser3", ChatId = 3, UserId = 3, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser4", ChatId = 3, UserId = 4, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser1", ChatId = 4, UserId = 1, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser2", ChatId = 4, UserId = 2, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser6", ChatId = 4, UserId = 6, IsActive = true, IsViewed = false, Time = DateTime.Now });
                messages.Add(new Message() { Text = "textuser7", ChatId = 4, UserId = 7, IsActive = true, IsViewed = false, Time = DateTime.Now });
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

                var username = _menu.SignIn();

                if (users.IsUserExist(username))
                {
                    if (users.UserHasChats(username))
                    {
                        var userChats = chats.GetAll().FindAll(i => i.Users.Contains(users.Get(username)));

                        foreach (var chat in userChats)
                        {
                            if (messages.IsChatNotEmpty(chat))
                            {
                                var lastMessage = messages.GetChatMessages(chat).Last();
                                var authorOfLastMessage = users.GetAll().Find(i => lastMessage.UserId == i.Id);
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

                users.Add(_menu.SignUp(users.GetAll()));
                users.SaveToDb();
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
                //var bots = null;
                //TODO create botmanager
                var message = _menu.AddMessage(messages, chats, users);

                messages.Add(message);
                messages.SaveToDb();
                chats.SaveToDb();
                users.SaveToDb();
            }

        }

        private void OpenChat()
        {
            var chatname = _menu.GetChatNameToOpen();
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var chat = chats.GetAll().Find(i => i.Name == chatname);
                var chatUsers = users.GetAll().FindAll(i => chat.Users.Contains(i));
                _menu.OpenChat(chat, messages.GetChatMessages(chat), chatUsers);
                _menu.ChatActions();
            }
        }

        private void DeleteChat()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                _menu.DeleteChat(chats);
                chats.SaveToDb();
            }

        }

        private void DeleteMessage()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                _menu.DeleteMessage(chats, messages);
                chats.SaveToDb();
                messages.SaveToDb();
            }
        }


        private void ExitChat() { }

        private void BotInvoke() { }

        private void CreateChat()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                chats.Add(_menu.CreateChat(users, chats));
                chats.SaveToDb();
                users.SaveToDb();
            }
        }

        public void Start()
        {
            _menu.ShowAuthorizationPage();
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
            }
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
    }
}