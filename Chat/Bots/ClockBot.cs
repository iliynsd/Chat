using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Bots
{
    public class ClockBot : IMessageBot
    {
        public const string Name = "ClockBot";
        private IMessageBotService _messageService;
        private IChatActionBotService _chatActionService;
        private Dictionary<string, Func<string>> _botCommands;

        public ClockBot(IMessageBotService messageService, IChatActionBotService chatActionService)
        {
            _messageService = messageService;
            _chatActionService = chatActionService;
            _botCommands = new Dictionary<string, Func<string>>()
             {
                 {"time",() => DateTime.Now.ToString()},
                 {"early",() => DateTime.Now.AddMinutes(5).ToString()},
                 {"clockbot", () => DateTime.Now.ToString()}
             };
        }

        public async Task OnMessage(Message message)
        {
            await Task.Factory.StartNew(() =>
             {
                 if (_botCommands.ContainsKey(message.Text))
                 {
                     Thread.Sleep(60000);
                     var botAnswer = _botCommands[message.Text]?.Invoke();
                     _messageService.AddMessage(Name, message.ChatId, botAnswer);
                     _chatActionService.AddChatAction(Name, message.ChatId, botAnswer);
                 }
             });
        }
    }
}
