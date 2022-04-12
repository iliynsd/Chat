using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Chat
{
    public record Request(string method, string parameters);

    public static class RequestParser
    {
        public static Request Parse(HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream))
            {
                var line = reader.ReadLine();
                string parameters = string.Empty;
                if (!string.IsNullOrEmpty(line))
                {
                    parameters = line;
                }
                return new Request(request.RawUrl, parameters);
            }
        }

        private static List<object> ParseParams(string bodyRequest)
        {
            var parameteres = bodyRequest.Split('&');
            var result = new List<object>();
            foreach (var param in parameteres)
            {
                result.Add(param.Split('=')[1]);
            }

            return result;
        }
    }
}
