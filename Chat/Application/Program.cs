using Chat.Dal;
using Chat.Repositories;
using Chat.Repositories.PostgresRepositories;
using Chat.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Chat
{

    class Program
    {
        static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().Services.GetRequiredService<Messenger>().Start();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, serviceCollection) => {
                    serviceCollection.Configure<Options>(hostContext.Configuration.GetSection(Options.Path).Bind);

                           serviceCollection.AddTransient<App>();
                           serviceCollection.AddSingleton(hostContext.Configuration);
                           serviceCollection.AddSingleton<Configuration>();
                          
                           serviceCollection.AddDbContext<DataContext>(options => options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));
                           serviceCollection.AddSingleton<Messenger>();
                           serviceCollection.AddSingleton<IMenu, ConsoleMenu>();
                           serviceCollection.AddTransient<IMessageRepository, PostgresMessageRepository>();
                           serviceCollection.AddTransient<IChatRepository, PostgresChatRepository>();
                           serviceCollection.AddTransient<IUserRepository, PostgresUserRepository>();
                           serviceCollection.AddTransient<IChatActionsRepository, PostgresChatActionsRepository>();

                           serviceCollection.AddHostedService<Messenger>();
                });
        }
    }
}