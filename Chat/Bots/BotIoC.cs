using Chat.Bots;
using Chat.BotServices;
using Chat.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Chat
{
    public class BotIoC : IBotIoC
    {
        private ServiceCollection services;
        private ServiceProvider provider;
        
        
        public BotIoC(IServiceProvider serviceProvider)
        {
            services = new ServiceCollection();

            services.AddSingleton<IMessageBotService, MessageBotService>((_) => new MessageBotService(serviceProvider.GetRequiredService<IMessageRepository>(), serviceProvider.GetRequiredService<IChatRepository>(), serviceProvider.GetRequiredService<IChatActionsRepository>(), serviceProvider.GetRequiredService<IUserRepository>()));
            services.AddSingleton<IChatActionBotService, ChatActionBotService>((_) => new ChatActionBotService(serviceProvider.GetRequiredService<IChatRepository>(), serviceProvider.GetRequiredService<IChatActionsRepository>()));
            services.AddSingleton<IGoToUrlBotService, GoToUrlBotService>((_) => new GoToUrlBotService(serviceProvider.GetRequiredService<IChatRepository>(), serviceProvider.GetRequiredService<IUserRepository>()));
            services.AddSingleton<IMessageBot, ClockBot>();
            services.AddSingleton<IMessageBot, BotUploader>();

            provider = services.BuildServiceProvider();
        }

        public T Get<T>() => provider.GetRequiredService<T>();

        public IEnumerable<T> GetServices<T>() => provider.GetServices<T>();
    }
}