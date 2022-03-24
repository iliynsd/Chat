using System;
using System.Collections.Generic;
using System.Linq;
using Chat.Repositories;
using Chat.Utils;

namespace Chat
{
    public class Messenger
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

            if (IsUserExist(username))
            {
                if (UserHasChats(username))
                {

                    
                    var chats = GetChats(username);
                        foreach (var chat in chats)
                        {
                            if (IsChatNotEmpty(chat))
                            {
                                var lastMessage = GetLastMessage(chat);
                                var authorOfLastMessage = GetAuthorOfLastMessage(lastMessage);
                                _menu.ShowChat(chat, authorOfLastMessage, lastMessage);
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
            var chat = _chats.GetAll().Find(i => i.Name == _menu.GetChatNameToOpen());
            var users = _users.GetAll().FindAll(i => chat.UserIds.Contains(i.Id));
            _menu.OpenChat(chat, GetChatMessages(chat), users);
        }

        private void DeleteChat(){}
        
        private void DeleteMessage(){}
        
        private void BotInvoke(){}

        public void Start(Options options)
        {
            _menu.ShowAuthorizationPage();
            _users.GetFromDb(options.PathToUsers);
            _chats.GetFromDb(options.PathToChats);
            _messages.GetFromDb(options.PathToMessages);
            var cmd = Console.ReadLine();
            while (cmd != "exit")
            {
                if (_functional.ContainsKey(cmd))
                {
                    _functional[cmd]?.Invoke();
                    _chats.SaveToDb(options.PathToChats);
                    _users.SaveToDb(options.PathToUsers);
                    _messages.SaveToDb(options.PathToMessages);
                }
                else
                {
                    _menu.InvalidOperation();
                }

                cmd = Console.ReadLine();
            }


            // _messages.SaveToDb(pathOptions.PathToMessages);
            
            //  _chatActions.SaveToDb(pathOptions.PathToChatActions);
        }

        private List<Chat> GetChats(string username) => _chats.GetAll().FindAll(i => i.UserIds.Contains(_users.Get(username).Id));
        
        public  Message GetLastMessage(Chat chat) => _messages.GetAll().GroupBy(i => i.ChatId == chat.Id).SelectMany(group => group).Last();

        private List<Message> GetChatMessages(Chat chat) => _messages.GetAll().GroupBy(i => i.ChatId == chat.Id).SelectMany(group => group).ToList();
        
        private User GetAuthorOfLastMessage(Message lastMessage) => _users.GetAll().Find(i => lastMessage.UserId == i.Id);
        
        private void CreateChat()
        {
            _chats.Add(_menu.CreateChat(_users.GetAll(), _chats.GetAll()));
        }

        private bool IsChatNotEmpty(Chat chat) => _messages.GetAll().Select(i => i.ChatId).Contains(chat.Id);
        
        private bool UserHasChats(string userName) => _users.Get(userName).ChatIds.Count > 0;
        
        private bool IsUserExist(string username) => _users.GetAll().Select(i => i.Name).Contains(username);
    }
}