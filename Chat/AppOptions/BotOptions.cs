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
}
