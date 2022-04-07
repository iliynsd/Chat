namespace Chat
{
    internal interface IMessageBot
    {
        public void OnMessage(Message message);
    }
}