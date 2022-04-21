using System.IO;
using System.Threading.Tasks;

namespace Chat
{
    public static class ResponseWriter
    {
        public static async Task WriteResponseAsync(string response, Stream stream)
        {
            using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(response);
        }
    }
}
