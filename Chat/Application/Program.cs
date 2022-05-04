using Chat.AppOptions;
using Chat.Bots;
using Chat.Dal;
using Chat.MapperConfigurations;
using Chat.Repositories;
using Chat.Repositories.PostgresRepositories;
using Chat.UI;
using Chat.Utils;
using Chat.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
            CreateHostBuilder(args).Build().Services.GetRequiredService<IMenu>().Start();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, serviceCollection) =>
                {

                    serviceCollection.AddDbContext<DataContext>(options =>
                        options.UseLazyLoadingProxies().ConfigureWarnings(w => w.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning)).UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    serviceCollection.Configure<Options>(hostContext.Configuration.GetSection(Options.ApplicationPath).Bind);

                    serviceCollection.AddSingleton<IMenu, WebMenu>();
                    serviceCollection.AddTransient<IMessageRepository, PostgresMessageRepository>();
                    serviceCollection.AddTransient<IChatRepository, PostgresChatRepository>();
                    serviceCollection.AddTransient<IUserRepository, PostgresUserRepository>();
                    serviceCollection.AddTransient<IChatActionsRepository, PostgresChatActionsRepository>();
                    serviceCollection.AddScoped<IHandler, RequestHandler>();
                    serviceCollection.AddSingleton<ServerHost>();
                    serviceCollection.AddSingleton<IBotIoC, BotIoC>();

                    serviceCollection.AddAutoMapper(typeof(UserMapperConfuguration), typeof(ChatMapperConfiguration), typeof(MessageMapperConfiguration));

                    serviceCollection.AddSingleton<Messenger>();
                });
        }
    }
}