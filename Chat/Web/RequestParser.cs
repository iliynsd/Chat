using System.Collections.Generic;
using System.IO;

namespace Chat
{
    public static class RequestParser
    {
        public static List<string> ParseParams(Stream request)
        {
            List<string> result = null;
            using (var reader = new StreamReader(request))
            {
                var line = reader.ReadLine();
                if (line is not null)
                {
                    var parameteres = line.Split('&');
                    result = new List<string>();

                    foreach (var param in parameteres)
                    {
                        result.Add(param.Split('=')[1]);
                    }
                }
            }

            return result;
        }


        public static List<string> ParseParams(string request)
        {
            var result = new List<string>();
            var parameteres = request.Split('&');

            foreach (var param in parameteres)
            {
                result.Add(param.Split('=')[1]);
            }

            return result;
        }
    }
}