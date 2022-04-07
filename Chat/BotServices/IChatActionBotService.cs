namespace Chat.Bots
{
    public interface IChatActionBotService
    {
        public void AddChatAction(string botName, int chatId, string action);
    }
}
