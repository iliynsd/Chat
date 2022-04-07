using Chat.Models;
using Chat.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Chat
{
    public class MessageBotService : IMessageBotService
    {
        private IServiceProvider _serviceProvider;

        public MessageBotService([FromServices] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void AddMessage(string botName, int chatId, string answer)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var bot = users.Get(botName);
                var chat = chats.GetChatById(chatId);

                if (chat.Users.Contains(bot))
                {
                    var message = new Message(bot.Id, chat.Id, answer);
                    messages.Add(message);
                    var action = new ChatAction(ChatActions.UserAddMessage(botName, chat.Name, answer));
                    actions.Add(action);
                }

                messages.Save();
                actions.Save();
            }
        }
    }
}