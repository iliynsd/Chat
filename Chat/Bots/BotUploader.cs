using Chat.BotServices;
using System.Threading;
namespace Chat.Bots
{
    class BotUploader : IMessageBot
    {
        public const string Name = "BotUploader";
        private IGoToUrlBotService _goToUrlBotService;

        public BotUploader(IGoToUrlBotService goToUrlBotService)
        {
            _goToUrlBotService = goToUrlBotService;
        }

        public void OnMessage(Message message)
        {
            Thread.Sleep(10000);
            var url = message.Text;
            if (url.Contains("http"))
            {
                _goToUrlBotService.GoToUrl(Name, message.ChatId, url);
            }
        }
    }
}
