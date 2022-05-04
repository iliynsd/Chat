using Chat.AppOptions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Bots
{
    public abstract class BaseBot<T1, T2> : IBotsInvoker<T1, T2>
    {
        protected static SemaphoreSlim semaphore;
        protected BotOptions _options;

        public BaseBot(IOptions<BotOptions> options)
        {
            _options = options.Value;
            semaphore = new SemaphoreSlim(_options.BotThreads);
        }

        public abstract Task Invoke(IEnumerable<T1> bots, T2 param);
    }
}
