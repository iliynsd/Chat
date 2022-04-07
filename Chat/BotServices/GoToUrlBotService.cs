using Chat.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Chat.BotServices
{
    class GoToUrlBotService : IGoToUrlBotService
    {
        private IServiceProvider _serviceProvider;

        public GoToUrlBotService([FromServices] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void GoToUrl(string botName, int chatId, string url)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chats = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var bot = users.Get(botName);
                var chat = chats.GetChatById(chatId);

                if (chat.Users.Contains(bot))
                {
                    new OpenQA.Selenium.Chrome.ChromeDriver().Navigate().GoToUrl(url);
                }
            }
        }
    }
}
