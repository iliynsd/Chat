using System.Threading.Tasks;

namespace Chat
{
    public interface IMessageBot
    {
        public Task OnMessage(Message message);
    }
}