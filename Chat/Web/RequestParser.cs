using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public static List<string> ParseJson(Stream request)
        {
            List<string> result = new List<string>();
            using (var reader = new StreamReader(request))
            {
                var json = reader.ReadToEnd();
                var data = JObject.Parse(json);
                
                if(data.ContainsKey("userName"))
                {
                    result.Add(data.Property("userName").Name);
                }
                if(data.ContainsKey("chatName"))
                {
                    result.Add(data.Property("chatName").Name);
                }
                if(data.ContainsKey("textofMessage"))
                {
                    result.Add(data.Property("textOfMessage").Name);
                }
            }
            return result;
        }
    }
}