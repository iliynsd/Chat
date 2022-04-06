using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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


                //HttpListenerResponse response = ctx.Response;





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
                                    .Select((p, i) =>Convert.ChangeType(strParams[i], p.ParameterType))
                                    .ToArray();

                object ret = method.Invoke(this, @params);

                string retstr = JsonConvert.SerializeObject(ret);


               /* var file = File.ReadAllText("SignIn.html");
                byte[] buffer = Encoding.UTF8.GetBytes(file);

                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();*/




            }

            listener.Stop();
            listener.Close();



        }


        [Mapping("signIn")]
        public void SignIn(string name)
        {

            var file = File.ReadAllText("SignIn.html");
byte[] buffer = Encoding.UTF8.GetBytes(file);
           var response = WebRequest.Create("http://localhost:8080/").GetResponse().GetResponseStream();

            response.Write(buffer, 0, file.Length);
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
