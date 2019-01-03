namespace EthereumCore.Extensions
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Middleware Extension methods
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds the ethereum service.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddEthereumService(this IServiceCollection services)
        {
            services.AddScoped<Services.Contracts.IEthereumService, Services.EthereumService>();

            return services;    
        }
    }
}
