using Chat.Dal;
using Chat.Repositories;
using Chat.Repositories.PostgresRepositories;
using Chat.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Chat
{

    class Program
    {
        static async Task Main(string[] args)
        {

            var serviceCollection = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName).AddJsonFile("appSettings.json", optional: false).AddEnvironmentVariables()
                .Build();

            Host.CreateDefaultBuilder(args).ConfigureServices(serviceCollection =>
                       {

                           serviceCollection.Configure<Options>(configuration.GetSection(Options.Path).Bind);

                           serviceCollection.AddTransient<App>();
                           serviceCollection.AddSingleton(configuration);
                           serviceCollection.AddSingleton<Configuration>();
                          
                           serviceCollection.AddDbContext<DataContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
                           serviceCollection.AddSingleton<Messenger>();
                           serviceCollection.AddSingleton<IMenu, ConsoleMenu>();
                           serviceCollection.AddTransient<IMessageRepository, PostgresMessageRepository>();
                           serviceCollection.AddTransient<IChatRepository, PostgresChatRepository>();
                           serviceCollection.AddTransient<IUserRepository, PostgresUserRepository>();
                           serviceCollection.AddTransient<IChatActionsRepository, PostgresChatActionsRepository>();

                           serviceCollection.AddHostedService<Messenger>();

                       }).Build().Services.GetRequiredService<Messenger>().Start();
        }
    }
}