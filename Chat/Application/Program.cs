using Chat.Bots;
using Chat.BotServices;
using Chat.Dal;
using Chat.Repositories;
using Chat.Repositories.PostgresRepositories;
using Chat.UI;
using Chat.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Services.GetRequiredService<IMenu>().Start();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, serviceCollection) =>
                {

                    serviceCollection.AddDbContext<DataContext>(options =>
                        options.UseLazyLoadingProxies().ConfigureWarnings(w => w.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning)).UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    serviceCollection.AddSingleton<IMenu, WebMenu>();
                    serviceCollection.AddTransient<IMessageRepository, PostgresMessageRepository>();
                    serviceCollection.AddTransient<IChatRepository, PostgresChatRepository>();
                    serviceCollection.AddTransient<IUserRepository, PostgresUserRepository>();
                    serviceCollection.AddTransient<IChatActionsRepository, PostgresChatActionsRepository>();


                    serviceCollection.AddScoped<IHandler, RequestHandler>();
                    serviceCollection.AddSingleton<ServerHost>();


                    serviceCollection.AddSingleton<Messenger>();
                });
        }

        public static void RegistrateBotServices()
        {
            var serviceBotCollection = new ServiceCollection();
            serviceBotCollection.AddSingleton<IMessageBot, ClockBot>();
            serviceBotCollection.AddSingleton<IMessageBot, BotUploader>();
            serviceBotCollection.AddSingleton<IMessageBotService, MessageBotService>();
            serviceBotCollection.AddSingleton<IChatActionBotService, ChatActionBotService>();
            serviceBotCollection.AddSingleton<IGoToUrlBotService, GoToUrlBotService>();
            serviceBotCollection.BuildServiceProvider();
        }
    }
    //TODO сделать отдельный контейнер для ботов, насытить интерфейс он должен принимать url вызывать соответсвующий метод у интерфейса для разборки урла вынимать все параметры и вызывать методы messenger api типа SignIn(username)
}//TODO сделать webmenu принимать url, возвращать json, потом вынести логику api в отдельный файл, переделать ботов, сделать userdto; 