namespace EthereumCore.App
{
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using EthereumCore.Extensions;

    /// <summary>
    /// Program entry point
    /// </summary>
    class Program
    {
        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                  .ConfigureAppConfiguration((hostingContext, config) =>
                  {
                      config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                      config.AddJsonFile($"appsettings.override.json", optional: true, reloadOnChange: true);

                  })
                  .ConfigureServices((hostContext, services) =>
                  {
                      services.AddOptions();
                      services.AddSingleton<IHostedService, AppService>();

                      services.Configure<Models.EthereumSettings>(config =>
                      {
                          config.EhtereumAccount = hostContext.Configuration["EthereumSettings:EhtereumAccount"];
                          config.EhtereumPassword = hostContext.Configuration["EthereumSettings:EhtereumPassword"];
                          config.StorageAccount = hostContext.Configuration["EthereumSettings:StorageAccount"];
                          config.StorageKey = hostContext.Configuration["EthereumSettings:StorageKey"];
                      });
                      services.AddEthereumService();

                  })
                  .ConfigureLogging((hostingContext, logging) => {
                      logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                      logging.AddConsole();
                  });

            await builder.RunConsoleAsync();
        }
    }
}
