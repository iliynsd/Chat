using System.Collections.Generic;
using System.IO;

namespace Chat
{
    public static class RequestParser
    {
        public static List<object> ParseParams(Stream request)
        {
            using (var reader = new StreamReader(request))
            {
                var line = reader.ReadLine();
                if (line is null)
                {
                    return null;
                }
                else
                {
                    var parameteres = line.Split('&');
                    var result = new List<object>();

                    foreach (var param in parameteres)
                    {
                        result.Add(param.Split('=')[1]);
                    }

                    return result;
                }

            }
        }

        public static List<object> ParseParams(string request)
        {
            var parameteres = request.Split('&');
            var result = new List<object>();

            foreach (var param in parameteres)
            {
                result.Add(param.Split('=')[1]);
            }

            return result;
        }

    }
}