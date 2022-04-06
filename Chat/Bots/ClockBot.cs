using Chat.Models;
using Chat.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Chat.Bots
{
    public class ClockBot : IObserver<ChatAction>
    {
        public const string Name = "ClockBot";
        private IServiceProvider _serviceProvider;
        private Dictionary<string, Func<string>> _botCommands;

        public ClockBot([FromServices] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _botCommands = new Dictionary<string, Func<string>>()
            {
                { "time",() => DateTime.Now.ToString()},
                {"early",() => DateTime.Now.AddMinutes(5).ToString()},
                {"clockbot", () => DateTime.Now.ToString()}
            };
        }
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(ChatAction value)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var messages = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var bot = users.Get(Name);
                if (value.ActionText.Contains("add message"))
                {
                    var actionText = value.ActionText.Split(' ');
                    var textOfMessage = actionText[actionText.Length - 2];
                    var chatname = value.ActionText.Split(' ')[4];
                    var chat = chats.GetChat(chatname);
                    if (chat.Users.Contains(bot))
                    {
                        if (_botCommands.ContainsKey(textOfMessage))
                        {
                            var botAnswer = _botCommands[textOfMessage]?.Invoke();
                            var message = new Message(bot.Id, chat.Id, botAnswer);
                            messages.Add(message);
                            var action = new ChatAction(ChatActions.UserAddMessage(Name, chatname, botAnswer));
                            actions.Add(action);
                        }
                    }
                }

                messages.SaveToDb();
                actions.SaveToDb();
            }
        }
    }
}
