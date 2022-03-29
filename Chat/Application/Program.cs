using Chat.Dal;
using Chat.Repositories;
using Chat.Repositories.PostgresRepositories;
using Chat.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Chat
{
    class Program
    {
        static async Task Main()
        {

            var serviceCollection = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName).AddJsonFile("appSettings.json", optional: false).AddEnvironmentVariables()
                .Build();


            serviceCollection.Configure<Options>(configuration.GetSection(Options.Path).Bind);

            serviceCollection.AddTransient<App>();
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddSingleton<Configuration>();

            serviceCollection.AddSingleton<Messenger>();
            serviceCollection.AddSingleton<IMenu, ConsoleMenu>();
           // serviceCollection.AddSingleton<IMessageRepository, MessageFileRepository>();
           // serviceCollection.AddSingleton<IChatRepository, ChatFileRepository>();
           // serviceCollection.AddSingleton<IUserRepository, UserFileRepository>();
            //serviceCollection.AddSingleton<IChatActionsRepository, ChatActionsFileRepository>();
            serviceCollection.AddTransient<IMessageRepository, PostgresMessageRepository>();
            serviceCollection.AddTransient<IChatRepository, PostgresChatRepository>();
            serviceCollection.AddTransient<IUserRepository, PostgresUserRepository>();
            serviceCollection.AddTransient<IChatActionsRepository, PostgresChatActionsRepository>();
            serviceCollection.AddEntityFrameworkNpgsql().AddDbContext<DataContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            

            var serviceProvider = serviceCollection.BuildServiceProvider();

           

            await serviceProvider.GetRequiredService<App>().Run(serviceProvider);
        }

    }
}