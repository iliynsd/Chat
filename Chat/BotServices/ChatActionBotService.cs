using Chat.Bots;
using Chat.Models;
using Chat.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Chat.BotServices
{
    public class ChatActionBotService : IChatActionBotService
    {
        private IServiceProvider _serviceProvider;

        public ChatActionBotService([FromServices] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void AddChatAction(string botName, int chatId, string text)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var chat = chats.GetChatById(chatId);
                var action = new ChatAction(ChatActions.BotAddMessage(botName, chat.Name, text));
                actions.Add(action);
                actions.Save();
            }
        }
    }
}
