namespace Chat
{
    public interface IMessageBotService
    {
        public void AddMessage(string botName, int chatId, string answer);
    }
}
