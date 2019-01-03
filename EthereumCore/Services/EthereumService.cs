namespace EthereumCore.Services
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Options;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Table;

    using Nethereum.Contracts;
    using Nethereum.Web3;
    using Nethereum.Util;

    using EthereumCore.Models;

    public class BasicEthereumService : Contracts.IEthereumService
    {
        private Web3 web3;

        private string password;

        private string storageKey;

        private string storageAccount;

        /// <summary>
        /// Gets or sets the account address.
        /// </summary>
        /// <value>
        /// The account address.
        /// </value>
        public string AccountAddress { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicEthereumService"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public BasicEthereumService(IOptions<EthereumSettings> config)
        {
            this.web3 = new Web3("http://localhost:8545");
            this.AccountAddress = config.Value.EhtereumAccount;
            this.password = config.Value.EhtereumPassword;
            this.storageAccount = config.Value.StorageAccount;
            this.storageKey = config.Value.StorageKey;
        }


        /// <summary>
        /// Saves the contract to table storage.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <returns></returns>
        public async Task<bool> SaveContractToTableStorageAsync(EthereumContract contract)
        {
            StorageCredentials credentials = new StorageCredentials(storageAccount, storageKey);
            CloudStorageAccount account = new CloudStorageAccount(credentials, true);
            var client = account.CreateCloudTableClient();

            var tableRef = client.GetTableReference("ethtransactions");
            await tableRef.CreateIfNotExistsAsync();

            TableOperation ops = TableOperation.InsertOrMerge(contract);
            await tableRef.ExecuteAsync(ops);
            return true;
        }

        /// <summary>
        /// Gets the contract from table storage.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public async Task<EthereumContract> GetContractFromTableStorageAsync(string name)
        {
            StorageCredentials credentials = new StorageCredentials(storageAccount, storageKey);
            CloudStorageAccount account = new CloudStorageAccount(credentials, true);
            var client = account.CreateCloudTableClient();

            var tableRef = client.GetTableReference("ethtransactions");
            await tableRef.CreateIfNotExistsAsync();

            TableOperation ops = TableOperation.Retrieve<EthereumContract>("contract", name);
            var tableResult = await tableRef.ExecuteAsync(ops);
            if (tableResult.HttpStatusCode == 200)
                return (EthereumContract)tableResult.Result;
            else
                return null;
        }

        /// <summary>
        /// Gets the balance.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public async Task<decimal> GetBalanceAsync(string address)
        {
            var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
            return UnitConversion.Convert.FromWei(balance.Value, 18);
        }

        /// <summary>
        /// Releases the contract.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="abi">The abi.</param>
        /// <param name="byteCode">The byte code.</param>
        /// <param name="gas">The gas.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Contract {name}</exception>
        public async Task<bool> ReleaseContractAsync(string name, string abi, string byteCode, int gas)
        {

            // check contractName
            var existing = await this.GetContractFromTableStorageAsync(name);
            if (existing != null) throw new Exception($"Contract {name} is present in storage");
            try
            {
                var resultUnlocking = await web3.Personal.UnlockAccount.SendRequestAsync(this.AccountAddress, password, 60);
                if (resultUnlocking)
                {
                    var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, this.AccountAddress, new Nethereum.Hex.HexTypes.HexBigInteger(gas), 2);

                    EthereumContract eci = new EthereumContract()
                    {
                        RowKey = name,
                        Abi = abi,
                        Bytecode = byteCode,
                        TransactionHash = transactionHash
                    };
                    return await SaveContractToTableStorageAsync(eci);
                }
            }
            catch (Exception exc)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Tries the get contract address.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Contract {name}</exception>
        public async Task<string> TryGetContractAddressAsync(string name)
        {
            // check contractName
            var existing = await this.GetContractFromTableStorageAsync(name);
            if (existing == null) throw new Exception($"Contract {name} does not exist in storage");

            if (!String.IsNullOrEmpty(existing.ContractAddress))
                return existing.ContractAddress;
            else
            {
                var resultUnlocking = await web3.Personal.UnlockAccount.SendRequestAsync(this.AccountAddress, password, 60);
                if (resultUnlocking)
                {
                    var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(existing.TransactionHash);
                    if (receipt != null)
                    {
                        existing.ContractAddress = receipt.ContractAddress;
                        await SaveContractToTableStorageAsync(existing);
                        return existing.ContractAddress;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the contract.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// Contract {name}
        /// or
        /// Contract address for {name}
        /// </exception>
        public async Task<Contract> GetContractAsync(string name)
        {
            var existing = await this.GetContractFromTableStorageAsync(name);
            if (existing == null) throw new Exception($"Contract {name} does not exist in storage");
            if (existing.ContractAddress == null) throw new Exception($"Contract address for {name} is empty. Please call TryGetContractAddress until it returns the address");

            var resultUnlocking = await web3.Personal.UnlockAccount.SendRequestAsync(this.AccountAddress, password, 60);
            if (resultUnlocking)
            {
                return web3.Eth.GetContract(existing.Abi, existing.ContractAddress);
            }
            return null;
        }
    }
}
