using Chat.Dal;
using Chat.Repositories;
using Chat.Repositories.PostgresRepositories;
using Chat.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<DataContext>(options => options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));
            serviceCollection.AddSingleton<Messenger>();
            serviceCollection.AddSingleton<IMenu, ConsoleMenu>();
            serviceCollection.AddTransient<IMessageRepository, PostgresMessageRepository>();
            serviceCollection.AddTransient<IChatRepository, PostgresChatRepository>();
            serviceCollection.AddTransient<IUserRepository, PostgresUserRepository>();
            serviceCollection.AddTransient<IChatActionsRepository, PostgresChatActionsRepository>();
        }

        public  void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
        }
    }
}