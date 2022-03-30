using Chat.Dal;
using Chat.Repositories;
using Chat.Repositories.PostgresRepositories;
using Chat.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Chat.Application;

namespace Chat
{
  
    class Program
    {
        static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();


            var serviceCollection = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName).AddJsonFile("appSettings.json", optional: false).AddEnvironmentVariables()
                .Build();


            serviceCollection.Configure<Options>(configuration.GetSection(Options.Path).Bind);

            serviceCollection.AddTransient<App>();
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddSingleton<Configuration>();
            serviceCollection.AddSingleton<Startup>();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args);
    }
}