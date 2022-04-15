using System.IO;

namespace Chat
{
    public static class ResponseWriter
    {
        public static void WriteResponse(object response, Stream stream)
        {
            using var writer = new StreamWriter(stream);
            writer.WriteLine(response);
        }
    }
}
