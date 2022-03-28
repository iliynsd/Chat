using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Chat
{
    public class App
    { 
        public async Task Run(IServiceProvider provider)
        {
            var messenger = provider.GetRequiredService<Messenger>();
            messenger.Start();
            await Task.CompletedTask;
        }
    }
}