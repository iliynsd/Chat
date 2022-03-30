using Chat.Repositories;
using Chat.Utils;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chat
{
    public class Messenger : IHostedService
    {
        private Dictionary<string, Action> _functional;
        private IMenu _menu;
        private IChatRepository _chats;
        private IMessageRepository _messages;
        private IUserRepository _users;
        private IChatActionsRepository _chatActions;

        public Messenger(IMenu menu, IChatRepository chats, IMessageRepository messages, IUserRepository users, IChatActionsRepository chatActions)
        {
            _functional = new Dictionary<string, Action>()
            {
                {"signIn", SignIn},
                {"signUp", SignUp},
                {"signOut", SignOut},
                {"create-chat", CreateChat},
                {"delete-chat", DeleteChat},
                {"open-chat", OpenChat},
                {"add-mes", AddMessage},
                {"del-mes", DeleteMessage},
                {"exit-chat", ExitChat},
                {"bot", BotInvoke}
            };

            _menu = menu;
            _chats = chats;
            _messages = messages;
            _users = users;
            _chatActions = chatActions;
        }

        private void SignIn()
        {
            var username = _menu.SignIn();

            if (_users.IsUserExist(username))
            {
                if (_users.UserHasChats(username))
                {
                    var chats = GetChats(username);

                    foreach (var chat in chats)
                    {
                        if (_messages.IsChatNotEmpty(chat))
                        {
                            var lastMessage = _messages.GetChatMessages(chat).Last();
                            var authorOfLastMessage = GetAuthorOfLastMessage(lastMessage);
                            _menu.ShowChatWithLastMessage(chat, lastMessage.Text, authorOfLastMessage);
                        }
                    }

                    _menu.ShowMainMenu();
                }
                else
                {
                    _menu.ShowFitstChatCreate();
                }
            }
            else
            {
                _menu.IncorrectUserName();
                _menu.ShowAuthorizationPage();
            }
        }

        private void SignUp()
        {
            _users.Add(_menu.SignUp(_users));
            _menu.SuccessSignUp();
        }

        private void SignOut()
        {
            _menu.SignOut();
            _menu.ShowAuthorizationPage();
        }

        private void AddMessage()
        {
            _messages.Add(_menu.AddMessage(_messages, _chats, _users));
        }

        private void OpenChat()
        {
            var chatname = _menu.GetChatNameToOpen();
            var chat = _chats.GetAll().Find(i => i.Name == chatname);
            var users = _users.GetAll().FindAll(i => chat.UserIds.Contains(i.Id));
            _menu.OpenChat(chat, _messages.GetChatMessages(chat), users);
            _menu.ChatActions();
        }

        private void DeleteChat() => _menu.DeleteChat(_chats);

        private void DeleteMessage() => _menu.DeleteMessage(_chats, _messages);

        private void ExitChat() { }

        private void BotInvoke() { }

        private void CreateChat() => _chats.Add(_menu.CreateChat(_users, _chats));

        public void Start()
        {
            _menu.ShowAuthorizationPage();
            _users.GetFromDb();
            _chats.GetFromDb();
            _messages.GetFromDb();
            var cmd = Console.ReadLine();
            while (cmd != "exit")
            {
                if (_functional.ContainsKey(cmd))
                {
                    _functional[cmd]?.Invoke();
                    _chats.SaveToDb();
                    _users.SaveToDb();
                    _messages.SaveToDb();
                }
                else
                {
                    _menu.InvalidOperation();
                }

                cmd = Console.ReadLine();
            }
        }

        private List<Chat> GetChats(string username) => _chats.GetAll().FindAll(i => i.UserIds.Contains(_users.Get(username).Id));

        private User GetAuthorOfLastMessage(Message lastMessage) => _users.GetAll().Find(i => lastMessage.UserId == i.Id);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}