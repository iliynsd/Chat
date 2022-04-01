using Chat.Dal;
using Chat.Repositories;
using Chat.Repositories.PostgresRepositories;
using Chat.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chat
{

    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, serviceCollection) =>
                {

                    serviceCollection.AddDbContext<DataContext>(options =>
                        options.UseLazyLoadingProxies().UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    serviceCollection.AddSingleton<IMenu, ConsoleMenu>();
                    //serviceCollection.AddSingleton<IMessageRepository, MessageFileRepository>();
                    //serviceCollection.AddSingleton<IChatRepository, ChatFileRepository>();
                    //serviceCollection.AddSingleton<IUserRepository, UserFileRepository>();

                    serviceCollection.AddTransient<IMessageRepository, PostgresMessageRepository>();
                    serviceCollection.AddTransient<IChatRepository, PostgresChatRepository>();
                    serviceCollection.AddTransient<IUserRepository, PostgresUserRepository>();

                    serviceCollection.AddHostedService<Messenger>();
                    // serviceCollection.AddTransient<IChatActionsRepository, PostgresChatActionsRepository>();
                });
        }
    }
}