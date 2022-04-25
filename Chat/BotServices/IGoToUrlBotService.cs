namespace Chat.BotServices
{
    public interface IGoToUrlBotService
    {
        public void GoToUrl(string botName, int chatId, string url);
    }
}
