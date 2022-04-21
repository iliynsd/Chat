using Chat.BotServices;
using System.Threading.Tasks;
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

        public async void OnMessage(Message message)
        {
            await Task.Factory.StartNew(() =>
            {
                var url = message.Text;
                if (url.Contains("http"))
                {
                    _goToUrlBotService.GoToUrl(Name, message.ChatId, url);
                }
            });
        }
    }
}
