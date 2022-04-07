namespace Chat.BotServices
{
    interface IGoToUrlBotService
    {
        public void GoToUrl(string botName, int chatId, string url);
    }
}
