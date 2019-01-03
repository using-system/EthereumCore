namespace EthereumCore.App
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EthereumCore.Services.Contracts;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// App Service
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    /// <seealso cref="System.IDisposable" />
    public class AppService : IHostedService, IDisposable
    {
        private ILogger logger;

        private IEthereumService ethereumService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppService" /> class.
        /// </summary>
        /// <param name="ethereumService">The ethereum service.</param>
        /// <param name="logger">The logger.</param>
        public AppService(IEthereumService ethereumService, ILogger<AppService> logger)
        {
            this.ethereumService = ethereumService;
            this.logger = logger;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("App started");

            await this.ExecuteUserCommand();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("App stopped");

            return Task.CompletedTask;
        }

        private async Task ExecuteUserCommand()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Command ?");

                    string command = Console.ReadLine();
                    switch (command)
                    {
                        case "getcontractaddress":
                            Console.WriteLine("Contract name");
                            string contractAddress = await this.ethereumService.TryGetContractAddressAsync(Console.ReadLine());
                            Console.WriteLine($"Contract address {contractAddress}");
                            break;
                        default:
                            continue;
                    }
                }
                catch(Exception exc)
                {
                    this.logger.LogError(exc, "An error onccured");
                }

            }           
        }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
        public void Dispose()
        {

        }
    }
}
