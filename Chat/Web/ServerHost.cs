using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Chat
{
    public class ServerHost
    {
        private IHandler _handler;
        private readonly AppOptions.Options _options;

        public ServerHost(IOptions<AppOptions.Options> options, IHandler handler)
        {
            _handler = handler;
            _options = options.Value;
        }

        public async Task StartAsync()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(_options.Protocol + _options.Host + _options.Port);
            listener.Start();

            while (true)
            {
                var context = listener.GetContext();
                var request =  context.Request;
                var response = context.Response;
                await _handler.HandleAsync(request, response);
            }
        }
    }
}
