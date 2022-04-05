namespace Chat.Models
{
    public class ChatActions
    {
        public static string UserSignIn(string username) => $"User {username} sign in";
        public static string UserSignUp(string username) => $"User {username} sign up";
        public static string UserAddMessage(string username, string chatname, string text) => $"User {username} in chat {chatname} add message {text} ";
        public static string UserDeleteMessage(string username, string chatname) => $"User {username} in chat {chatname} delete message";
        public static string UserCreateChat(string username, string chatname) => $"User {username} create chat - {chatname}";
        public static string UserDeleteChat(string username, string chatname) => $"User {username} delete chat - {chatname}";
        public static string UserLeftChat(string username, string chatname) => $"User {username} left chat - {chatname}";
        public static string UserSignOut(string username) => $"User {username} sign out";
    }
}
