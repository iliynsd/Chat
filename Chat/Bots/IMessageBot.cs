using System.Threading.Tasks;

namespace Chat
{
    internal interface IMessageBot
    {
        public Task OnMessage(Message message);
    }
}