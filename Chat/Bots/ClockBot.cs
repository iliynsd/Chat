using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Bots
{
    public class ClockBot : IMessageBot
    {
        public const string Name = "ClockBot";
        private readonly IMessageBotService _messageService;
        private readonly IChatActionBotService _chatActionService;
        private Dictionary<string, Func<string>> _botCommands;

        public ClockBot(IMessageBotService messageService, IChatActionBotService chatActionService)
        {
            _botCommands = new Dictionary<string, Func<string>>()
             {
                 {"time",() => DateTime.Now.ToString()},
                 {"early",() => DateTime.Now.AddMinutes(5).ToString()},
                 {"clockbot", () => DateTime.Now.ToString()}
             };
            _messageService = messageService;
            _chatActionService = chatActionService;
        }

        public Task OnMessage(Message message)
        {
            if (_botCommands.ContainsKey(message.Text))
            {
                Thread.Sleep(10000);
                var botAnswer = _botCommands[message.Text]?.Invoke();
                _messageService.AddMessage(Name, message.ChatId, botAnswer);
                _chatActionService.AddChatAction(Name, message.ChatId, botAnswer);
            }
            return Task.CompletedTask;
        }
    }
}
