using Chat.BotServices;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Bots
{
   public class BotUploader : IMessageBot
    {
        public const string Name = "BotUploader";
        private readonly IGoToUrlBotService _goToUrlBotService;

        public BotUploader(IGoToUrlBotService parGoToUrlBotService)
        {
            _goToUrlBotService = parGoToUrlBotService;
        }

        public Task OnMessage(Message message)
        {
            Thread.Sleep(10000);
            var url = message.Text;
            if (url.Contains("http"))
            {
                _goToUrlBotService.GoToUrl(Name, message.ChatId, url);
            }
            return Task.CompletedTask;
        }
    }
}
