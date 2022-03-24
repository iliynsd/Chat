using System;

namespace Chat.UI
{
    public class UIConsole
    {
        public static void ShowChatWithLastMessage(Chat chat, string message, User user)
        {
            Console.WriteLine(chat.Name);
            Console.WriteLine("By " + user.Type + user.Name + message);
        }
    }
}