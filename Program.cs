using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chat_Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    var env = context.HostingEnvironment;

                    configurationBuilder.SetBasePath(env.ContentRootPath);
                    configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    configurationBuilder.AddConfiguration(context.Configuration);
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Service>();
                });
    }
}