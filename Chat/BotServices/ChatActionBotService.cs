using Chat.Bots;
using Chat.Models;
using Chat.Repositories;

namespace Chat.BotServices
{
    public class ChatActionBotService : IChatActionBotService
    {
        private IChatRepository _chats;
        private IChatActionsRepository _chatActions;

        public ChatActionBotService(IChatRepository chats, IChatActionsRepository chatActions)
        {
            _chats = chats;
            _chatActions = chatActions;
        }

        public void AddChatAction(string botName, int chatId, string text)
        {
            var chat = _chats.GetChatById(chatId);
            var action = new ChatAction(ChatActions.BotAddMessage(botName, chat.Name, text));
            _chatActions.Add(action);
            _chatActions.Save();
        }
    }
}