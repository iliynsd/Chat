using System.Net;

namespace Chat
{
    public class ServerHost
    {
        private IHandler _handler;
        public ServerHost(IHandler handler)
        {
            _handler = handler;
        }

        public void Start()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:80/");
            listener.Start();

            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;

                var response = context.Response;
                _handler.Handle(request, response);
            }
        }
    }
}
