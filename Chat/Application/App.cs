using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

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