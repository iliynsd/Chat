using Chat.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Chat.UI
{
    public class WebMenu : IMenu
    {
        private IServiceProvider _serviceProvider;

        public WebMenu(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async void Start()
        {
            var server = _serviceProvider.GetRequiredService<ServerHost>();
            await server.StartAsync();
        }
    }
}