using Chat.Models;
using Chat.Repositories;

namespace Chat
{
    public class MessageBotService : IMessageBotService
    {
        private IMessageRepository _messages;
        private IChatRepository _chats;
        private IChatActionsRepository _chatActions;
        private IUserRepository _users;

        public MessageBotService(IMessageRepository messages, IChatRepository chats, IChatActionsRepository chatActions, IUserRepository users)
        {
            _messages = messages;
            _chats = chats;
            _chatActions = chatActions;
            _users = users;
        }

        public void AddMessage(string botName, int chatId, string answer)
        {
            var bot = _users.Get(botName);
            var chat = _chats.GetChatById(chatId);

            if (chat.Users.Contains(bot))
            {
                var message = new Message(bot.Id, chat.Id, answer);
                _messages.Add(message);
                var action = new ChatAction(ChatActions.UserAddMessage(botName, chat.Name, answer));
                _chatActions.Add(action);
            }

            _messages.Save();
            _chatActions.Save();
        }
    }
}