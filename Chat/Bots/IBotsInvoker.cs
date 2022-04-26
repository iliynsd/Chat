using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Bots
{
    public interface IBotsInvoker<T1, T2>   
    {
        public Task Invoke(IEnumerable<T1> bots, T2 param);
    }
}