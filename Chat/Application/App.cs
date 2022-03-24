using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Chat
{
    public class App
    {
        private readonly Options _options;

        public App(IOptions<Options> options)
        {
            _options = options.Value;
        }

        public async Task Run(IServiceProvider provider)
        {
            var messenger = provider.GetRequiredService<Messenger>();
            messenger.Start(_options);
            await Task.CompletedTask;
        }
    }
}