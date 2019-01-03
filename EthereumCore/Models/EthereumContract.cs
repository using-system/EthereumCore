namespace EthereumCore.Models
{
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Ethereum Contract
    /// </summary>
    /// <seealso cref="Microsoft.WindowsAzure.Storage.Table.TableEntity" />
    public class EthereumContract : TableEntity
    {
        /// <summary>
        /// Gets or sets the abi.
        /// </summary>
        /// <value>
        /// The abi.
        /// </value>
        public string Abi { get; set; }

        /// <summary>
        /// Gets or sets the bytecode.
        /// </summary>
        /// <value>
        /// The bytecode.
        /// </value>
        public string Bytecode { get; set; }

        /// <summary>
        /// Gets or sets the transaction hash.
        /// </summary>
        /// <value>
        /// The transaction hash.
        /// </value>
        public string TransactionHash { get; set; }

        /// <summary>
        /// Gets or sets the contract address.
        /// </summary>
        /// <value>
        /// The contract address.
        /// </value>
        public string ContractAddress { get; set; }
    }
}
