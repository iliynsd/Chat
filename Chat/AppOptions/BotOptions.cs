using Chat.Bots;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.AppOptions
{
   public class BotOptions 
    {
        public const string Threads = "threadsForBotsAmount";
        public int BotThreads { get; set; }
    }

    public abstract class BaseBot<T1, T2> : IBotsInvoker<T1, T2>
    {
        public static SemaphoreSlim semaphore;
        public BotOptions _options;
        
        public BaseBot(IOptions<BotOptions> options)    
        {
            _options = options.Value;
            semaphore = new SemaphoreSlim(_options.BotThreads);
        }

        public abstract Task Invoke(IEnumerable<T1> bots, T2 param);
    }
}
