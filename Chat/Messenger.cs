using Chat.Models;
using Chat.Repositories;
using Chat.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public class Messenger : IHostedService
    {
        private IMenu _menu;
        private IServiceProvider _serviceProvider;

        public Messenger(IMenu menu, [FromServices] IServiceProvider serviceProvider)
        {
            _menu = menu;
            _serviceProvider = serviceProvider;
        }


        public void SignIn(string userName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                if (users.IsUserExist(userName))
                {
                    var action = new ChatAction(ChatActions.UserSignIn(userName));
                    actions.Add(action);
                    actions.Save();
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

        public void SignUp()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var user = _menu.SignUp();

                if (users.IsUserExist(user.Name))
                {
                    _menu.UserExists();
                    user = _menu.SignUp();
                }



                users.Add(user);
                users.Save();

                var action = new ChatAction(ChatActions.UserSignUp(user.Name));
                actions.Add(action);
                actions.Save();

                _menu.SuccessSignUp();
            }
        }

        public void SignOut(string userName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                actions.Add(new ChatAction(ChatActions.UserSignOut(userName)));
                actions.Save();
            }
            _menu.SignOut();
            _menu.ShowAuthorizationPage();
        }

        public void AddMessage(string userName, string chatName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();


                var user = users.Get(userName);
                var message = _menu.AddMessage(user, chats.GetChat(chatName));

                messages.Add(message);
                var action = new ChatAction(ChatActions.UserAddMessage(userName, chatName, message.Text));

                actions.Add(action);
                actions.Save();
                messages.Save();
                var bots = scope.ServiceProvider.GetServices<IMessageBot>();

                foreach (var bot in bots)
                {
                    bot.OnMessage(message);
                }
            }

            _menu.ChatActions();
        }

        public void OpenChat(string userName, string chatName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var chat = chats.GetChat(chatName);
                messages.GetChatMessages(chat).ForEach(i => i.IsViewed = true);
                messages.Save();
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

        public void DeleteChat(string userName, string chatName)
        {
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

                actions.Save();
                chats.Save();
            }

            _menu.ShowMainMenu();
        }

        public void DeleteMessage(string userName, string chatName, string textOfMessage)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

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

                actions.Save();
                chats.Save();
                messages.Save();
            }

            _menu.ChatActions();
        }


        public void ExitChat(string userName, string chatName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var action = new ChatAction(ChatActions.UserLeftChat(userName, chatName));
                actions.Add(action);
                actions.Save();
            }

            _menu.ShowMainMenu();
        }

        public void CloseChat()
        {
            _menu.ShowMainMenu();
        }

        public void CreateChat()
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
                actions.Save();
                chats.Save();
                users.Save();
            }

            _menu.ShowMainMenu();
        }

        public void AddUserToChat(string userName, string chatName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var chat = chats.GetChat(chatName);
                var user = users.Get(userName);
                chat.Users.Add(user);
                chats.Save();
            }

            _menu.ChatActions();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _menu.ShowAuthorizationPage();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}