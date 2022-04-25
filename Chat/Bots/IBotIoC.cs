using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Bots
{
   public interface IBotIoC
    {
        public T Get<T>();
        public IEnumerable<T> GetServices<T>();
    }
}
