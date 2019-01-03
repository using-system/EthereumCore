namespace EthereumCore.Services.Contracts
{
    using System.Threading.Tasks;

    using Nethereum.Contracts;

    using EthereumCore.Models;

    public interface IEthereumService
    {
        /// <summary>
        /// Saves the contract to table storage.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <returns></returns>
        Task<bool> SaveContractToTableStorageAsync(EthereumContract contract);

        /// <summary>
        /// Gets the contract from table storage.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Task<EthereumContract> GetContractFromTableStorageAsync(string name);

        /// <summary>
        /// Gets the balance.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        Task<decimal> GetBalanceAsync(string address);

        /// <summary>
        /// Releases the contract.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="abi">The abi.</param>
        /// <param name="byteCode">The byte code.</param>
        /// <param name="gas">The gas.</param>
        /// <returns></returns>
        Task<bool> ReleaseContractAsync(string name, string abi, string byteCode, int gas);

        /// <summary>
        /// Tries the get contract address.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Task<string> TryGetContractAddressAsync(string name);

        /// <summary>
        /// Gets the contract.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Task<Contract> GetContractAsync(string name);
    }
}
