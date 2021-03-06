using Chat.Bots;
using Chat.Models;
using Chat.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public class Messenger
    {
        private IServiceProvider _serviceProvider;
        private static SemaphoreSlim _semaphoreBots;
        private AppOptions.Options _options;

        public Messenger(IServiceProvider serviceProvider, IOptions<AppOptions.Options> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _semaphoreBots = new SemaphoreSlim(2);
        }


        public List<Chat> SignIn(string userName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                if (users.IsUserExist(userName))
                {
                    var action = new ChatAction(ChatActions.UserSignIn(userName));
                    actions.Add(action);
                    actions.Save();
                    var userChats = chats.GetAll().FindAll(i => i.Users.Contains(users.Get(userName)));

                    return userChats;
                }
                return null;
            }
        }

        public bool SignUp(User user)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                if (users.IsUserExist(user.Name))
                {
                    return false;
                }
                else
                {

                    users.Add(user);
                    users.Save();
                    var action = new ChatAction(ChatActions.UserSignUp(user.Name));
                    actions.Add(action);
                    actions.Save();
                    return true;
                }
            }
        }

        public async Task<(Chat chat, List<Message> messages)> AddMessage(string userName, string chatName, string textOfMessage)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var user = users.Get(userName);
                var chat = chats.GetChat(chatName);

                textOfMessage = TryConvertUrl(textOfMessage);

                var message = new Message(user.Id, chat.Id, textOfMessage);

                messages.Add(message);
                var action = new ChatAction(ChatActions.UserAddMessage(userName, chatName, message.Text));

                actions.Add(action);
                actions.Save();
                messages.Save();

                var botIoc = _serviceProvider.GetRequiredService<IBotIoC>();
                var bots = botIoc.GetServices<IMessageBot>().ToList();

                var botInvoker = botIoc.Get<IBotsInvoker<IMessageBot, Message>>();
                await botInvoker.Invoke(bots, message);

                return (chat, messages.GetChatMessages(chat));
            }
        }

        public (Chat chat, List<Message> messages) OpenChat(string userName, string chatName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                var chat = chats.GetChat(chatName);
                chat.Users.Count();
                messages.GetChatMessages(chat).ForEach(i => i.IsViewed = true);
                messages.Save();
                
                var actionUserLeftChat = actions.Get($"User {userName} left chat - {chat.Name}");

                var actualMessages = messages.GetChatMessages(chat);

                if (actionUserLeftChat != null)
                {
                    actualMessages = messages.GetChatMessages(chat).FindAll(i => i.Time < actionUserLeftChat.Time);
                }

                return (chat, actualMessages);
            }
        }

        public List<Chat> DeleteChat(string userName, string chatName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var chat = chats.GetChat(chatName);
                var userChats = chats.GetAll().FindAll(i => i.Users.Contains(users.Get(userName)));

                chats.Delete(chat);
                var action = new ChatAction(ChatActions.UserDeleteChat(userName, chatName));
                actions.Add(action);
                actions.Save();
                chats.Save();
                return chats.GetAll().FindAll(i => i.Users.Contains(users.Get(userName)));
            }
        }

        public bool DeleteMessage(string userName, string chatName, int messageId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var chat = chats.GetChat(chatName);
                var message = messages.GetChatMessages(chat).Find(i => i.Id == messageId);
                var result = false;
                if (message != null && message.Time.AddDays(1) >= DateTime.Now)
                {
                    result = messages.Delete(message);
                    var action = new ChatAction(ChatActions.UserDeleteMessage(userName, chatName));
                    actions.Add(action);
                }

                actions.Save();
                chats.Save();
                messages.Save();
                return result;
            }
        }

        public List<Chat> ExitChat(string userName, string chatName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var action = new ChatAction(ChatActions.UserLeftChat(userName, chatName));
                actions.Add(action);
                actions.Save();
                return chats.GetAll().FindAll(i => i.Users.Contains(users.Get(userName)));
            }
        }

        public List<Chat> CreateChat(string userName, Chat chat)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                chat.Users.Add(users.Get(userName));
                chat.Users.Count();
                var action = new ChatAction(ChatActions.UserCreateChat(chat.Users.First().Name, chat.Name));
                actions.Add(action);
                chats.Add(chat);
                actions.Save();
                chats.Save();
                users.Save();
                return chats.GetAll().FindAll(i => i.Users.Contains(users.Get(userName)));
            }
        }

        public Chat AddUserToChat(string userName, string chatName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var chat = chats.GetChat(chatName);
                var user = users.Get(userName);
                chat.Users.Add(user);
                chat.Users.Count();
                chats.Save();
                return chat;
            }
        }

        private string TryConvertUrl(string url)
        {
            if (url.Contains("%3A%2F%2F"))
            {
                url = url.Replace("%3A%2F%2F", "://");
            }
            return url;
        }
    }
}