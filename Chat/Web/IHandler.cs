using System.Net;
using System.Threading.Tasks;

namespace Chat
{
    public interface IHandler
    {
        public Task HandleAsync(HttpListenerRequest request, HttpListenerResponse response);
    }
}
