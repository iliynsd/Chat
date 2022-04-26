using System.Collections.Generic;

namespace Chat.Bots
{
    public interface IBotIoC
    {
        public T Get<T>();
        public IEnumerable<T> GetServices<T>();
    }
}
