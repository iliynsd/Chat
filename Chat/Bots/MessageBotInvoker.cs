using Chat.AppOptions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Bots
{
    public class MessageBotInvoker : BaseBot<IMessageBot, Message>
    {
       public MessageBotInvoker(IOptions<BotOptions> options) : base(options)
        {

        }
        
        public override Task Invoke(IEnumerable<IMessageBot> bots, Message message)
        {
            foreach (var bot in bots)
            {
                var task = new Task(async () =>
                {
                    await semaphore.WaitAsync();
                    await bot.OnMessage(message);
                    semaphore.Release();
                });

                task.Start();
            }

            return Task.CompletedTask;
        }

        
    }
}
