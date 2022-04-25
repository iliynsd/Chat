using Chat.Repositories;

namespace Chat.BotServices
{
    class GoToUrlBotService : IGoToUrlBotService
    {
        private IChatRepository _chats;
        private IUserRepository _users;

        public GoToUrlBotService(IChatRepository chats, IUserRepository users)
        {
            _chats = chats;
            _users = users;
        }

        public void GoToUrl(string botName, int chatId, string url)
        {
            var bot = _users.Get(botName);
            var chat = _chats.GetChatById(chatId);

            if (chat.Users.Contains(bot))
            {
                new OpenQA.Selenium.Chrome.ChromeDriver().Navigate().GoToUrl(url);
            }
        }
    }
}