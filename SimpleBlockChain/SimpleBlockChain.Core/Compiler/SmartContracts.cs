using LevelDB;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class SmartContracts : IDisposable
    {
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly Networks _network;
        private DB _db;
        private const string _databaseFile = "sc_{0}.dat";
        private IEnumerable<SmartContract> _embeddedContracts = new List<SmartContract>
        {
            new SmartContract
            {
                Address = "0000000000000000000000000000000000000001".FromHexString(),
                Code = "60606040526000357c0100000000000000000000000000000000000000000000000000000000900480636d4ce63c1461003957610037565b005b61004660048050506100b4565b60405180806020018281038252838181518152602001915080519060200190808383829060006004602084601f0104600302600f01f150905090810190601f1680156100a65780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b6020604051908101604052806000815260200150604060405190810160405280600b81526020017f68656c6c6f20776f726c640000000000000000000000000000000000000000008152602001509050610109565b9056".FromHexString()
            }
        };

        private const string SMART_CONTRACT = "SMART_CONTRACT";
        private const string SMART_CONTRACT_ELT = SMART_CONTRACT + "_{0}";

        private const string SMART_CONTRACT_TX = "SMART_CONTRACT_TX";
        private const string SMART_CONTRACT_TX_ELT = SMART_CONTRACT_TX + "_{0}";

        private const string SMART_CONTRACT_STORE = "SMART_CONTRACT_STORE";
        private const string SMART_CONTRACT_STORE_ELT = SMART_CONTRACT_STORE + "_{0}_{1}";

        internal SmartContracts(IAssemblyHelper assemblyHelper, Networks network)
        {
            _assemblyHelper = assemblyHelper;
            _network = network;
            var options = new Options { CreateIfMissing = true };
            _db = new DB(GetDbFile(), options);
            Init();
        }

        public bool AddBlock(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            Persist(block);
            return true;
        }

        public SmartContract GetSmartContract(IEnumerable<byte> smartContractAddress)
        {
            if (smartContractAddress == null)
            {
                throw new ArgumentNullException(nameof(smartContractAddress));
            }

            string result = string.Empty;
            if (!_db.TryGet(string.Format(SMART_CONTRACT_ELT, smartContractAddress.ToHexString()), ReadOptions.Default, out result))
            {
                return null;
            }

            return new SmartContract
            {
                Address = smartContractAddress,
                Code = result.FromHexString()
            };
        }

        public IEnumerable<byte> GetSmartContractAddress(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            string result = string.Empty;
            if (!_db.TryGet(string.Format(SMART_CONTRACT_TX_ELT, txId.ToHexString()), ReadOptions.Default, out result))
            {
                return null;
            }

            return result.FromHexString();
        }

        public DataWord GetStorageRow(IEnumerable<byte> scAddr, DataWord key)
        {
            if (scAddr == null)
            {
                throw new ArgumentNullException(nameof(scAddr));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            
            var result = string.Empty;
            var scAddrHex = scAddr.ToHexString();
            var keyHex = key.GetData().ToHexString();
            if (!_db.TryGet(string.Format(SMART_CONTRACT_STORE_ELT, scAddrHex, keyHex), ReadOptions.Default, out result))
            {
                return DataWord.ZERO;
            }

            return new DataWord(result.FromHexString().ToArray());
        }

        public bool AddStorageRow(IEnumerable<byte> scAddr, DataWord key, DataWord value)
        {
            if (scAddr == null)
            {
                throw new ArgumentNullException(nameof(scAddr));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }


            string result = string.Empty;
            var scAddrHex = scAddr.ToHexString();
            var keyHex = key.GetData().ToHexString();
            var valueHex = value.GetData().ToHexString();
            if (!_db.TryGet(string.Format(SMART_CONTRACT_ELT, scAddrHex), ReadOptions.Default, out result))
            {
                return false;
            }

            result = string.Empty;
            if (_db.TryGet(string.Format(SMART_CONTRACT_STORE_ELT, scAddrHex, keyHex), ReadOptions.Default, out result))
            {
                _db.Delete(string.Format(SMART_CONTRACT_STORE_ELT, scAddrHex, keyHex));
            }

            _db.Put(string.Format(SMART_CONTRACT_STORE_ELT, scAddrHex, keyHex), valueHex);
            return true;
        }

        private void Persist(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var defaultCallValue = new DataWord(new byte[] { 0x00 });
            var batch = new WriteBatch();
            foreach (var transaction in block.Transactions)
            {
                var smartContractTransaction = transaction as SmartContractTransaction;
                if (smartContractTransaction == null)
                {
                    continue;
                }

                if (smartContractTransaction.To == null || !smartContractTransaction.To.Any()) // Create the contract.
                {
                    var solidityVm = new SolidityVm();
                    var program = new SolidityProgram(smartContractTransaction.Data.ToList(), new SolidityProgramInvoke(new byte[0], new DataWord(smartContractTransaction.From.ToArray()), defaultCallValue, this));
                    while(!program.IsStopped())
                    {
                        solidityVm.Step(program);
                    }

                    var contractCode = program.GetResult().GetHReturn();
                    var txId = transaction.GetTxId();
                    var smartContractAdr = smartContractTransaction.GetSmartContractAddress();
                    batch.Put(string.Format(SMART_CONTRACT_ELT, smartContractAdr.ToHexString()), contractCode.ToHexString());
                    batch.Put(string.Format(SMART_CONTRACT_TX_ELT, txId.ToHexString()), smartContractAdr.ToHexString());
                }
            }

            _db.Write(batch, WriteOptions.Default);
        }
        
        public void Persist(SmartContract smartContract)
        {
            if (smartContract == null)
            {
                throw new ArgumentNullException(nameof(smartContract));
            }

            var batch = new WriteBatch();
            batch.Put(string.Format(SMART_CONTRACT_ELT, smartContract.Address.ToHexString()), smartContract.Code.ToHexString());
            _db.Write(batch, WriteOptions.Default);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        private string GetDbFile()
        {
            var path = Path.GetDirectoryName(_assemblyHelper.GetEntryAssembly().Location);
            return Path.Combine(path, string.Format(_databaseFile, GetDirectoryName(_network)));
        }

        public static string GetDirectoryName(Networks networkEnum)
        {
            var network = "mainnet";
            switch (networkEnum)
            {
                case Networks.TestNet:
                    network = "testnet";
                    break;
                case Networks.RegTest:
                    network = "regtest";
                    break;
            }

            return network;
        }

        private void Init()
        {
            foreach(var embeddedContract in _embeddedContracts)
            {
                string result = string.Empty;
                if (!_db.TryGet(string.Format(SMART_CONTRACT_ELT, embeddedContract.Address.ToHexString()), ReadOptions.Default, out result))
                {
                    Persist(embeddedContract);
                }
            }
        }
    }
}
