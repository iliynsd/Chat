using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public class MessengerWeb : IHostedService
    {
        public void Start()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            while (true)
            {
                HttpListenerContext ctx = listener.GetContext();

                HttpListenerRequest request = ctx.Request;
                HttpListenerResponse response = ctx.Response;




                string methodName = ctx.Request.Url.Segments[1].Replace("/", "");
                string[] strParams = ctx.Request.Url
                                        .Segments
                                        .Skip(2)
                                        .Select(s => s.Replace("/", ""))
                                        .ToArray();

                var methods = GetType()
                                    .GetMethods()
                                    .Where(mi => mi.GetCustomAttributes(true).Any(attr => attr is Mapping && ((Mapping)attr).Map == methodName));
                Console.WriteLine(methods.Count());
                var method = methods.First();


                object[] @params = method.GetParameters()
                                    .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                                    .ToArray();

                object ret = method.Invoke(this, @params);
            }

            listener.Stop();
            listener.Close();
        }

        [Mapping("signIn")]
        public string SignIn(string username)
        {
            return username;
        }

        [Mapping("signUp")]
        public string SignUp()
        {
            return "SignIn.html";
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => Start());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }


    class Mapping : Attribute
    {
        public string Map;
        public Mapping(string s)
        {
            Map = s;
        }
    }
}
