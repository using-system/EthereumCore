namespace EthereumCore.App
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using EthereumCore.Models;
    using EthereumCore.Services.Contracts;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// App Service
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    /// <seealso cref="System.IDisposable" />
    public class AppService : IHostedService, IDisposable
    {
        private IEthereumService ethereumService;

        private string accountAddress;

        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppService" /> class.
        /// </summary>
        /// <param name="ethereumService">The ethereum service.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public AppService(IEthereumService ethereumService,
            IOptions<EthereumSettings> config,
            ILogger<AppService> logger)
        {
            this.ethereumService = ethereumService;
            this.accountAddress = config.Value.EhtereumAccount;
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
                        case "get":
                            Console.WriteLine("Contract name ?");
                            string contractAddress = await this.ethereumService.TryGetContractAddressAsync(Console.ReadLine());
                            Console.WriteLine($"Contract address {contractAddress}");
                            break;
                        case "release":
                            Console.WriteLine("Contract name ?");
                            string contractNameToRelease = Console.ReadLine();
                            Console.WriteLine("Abi ?");
                            string abiToRelease = Console.ReadLine();
                            Console.WriteLine("Bytecode ?");
                            string byteCodeToRelease = Console.ReadLine();
                            Console.WriteLine("Gas ?");
                            int.TryParse(Console.ReadLine(), out int gasToRelease);
                            bool isReleaseSucess = await this.ethereumService.ReleaseContractAsync(contractNameToRelease, abiToRelease,
                                byteCodeToRelease, gasToRelease);
                            Console.WriteLine($"Release contract result : {isReleaseSucess}");
                            break;
                        case "execute":
                            Console.WriteLine("Contract name ?");
                            string contractNameToExecute = Console.ReadLine();
                            Console.WriteLine("Contract method ?");
                            string contractMethodToExecute = Console.ReadLine();
                            Console.WriteLine("Value ?");
                            string contractMethodValue = Console.ReadLine();

                            string contractAddressToExecute = await this.ethereumService.TryGetContractAddressAsync(contractNameToExecute);
                            var contractToExecute = await this.ethereumService.GetContractAsync(contractNameToExecute);
                            var methodToExecute = contractToExecute.GetFunction(contractMethodToExecute);
                            var result = await methodToExecute.SendTransactionAsync(this.accountAddress, contractMethodValue);
                            Console.WriteLine($"Method result : {result}");
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
