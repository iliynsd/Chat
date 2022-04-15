using System.Net;

namespace Chat
{
    public interface IHandler
    {
        public void Handle(HttpListenerRequest request, HttpListenerResponse response);
    }
}
