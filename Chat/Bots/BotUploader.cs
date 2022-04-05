using Chat.Models;
using Chat.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Chat.Bots
{
    class BotUploader : IObserver<ChatAction>
    {
        public const string Name = "BotUploader";
        private IServiceProvider _serviceProvider;

        public BotUploader([FromServices] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
                var actions = scope.ServiceProvider.GetRequiredService<IChatActionsRepository>();

                if (value.ActionText.Contains("add message"))
                {
                    var actionText = value.ActionText.Split(' ');
                    var url = actionText[actionText.Length - 2];
                    if (url.Contains("http"))
                    {
                        new OpenQA.Selenium.Chrome.ChromeDriver().Navigate().GoToUrl(url);
                    }
                }
            }
        }
    }
}
