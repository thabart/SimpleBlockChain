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
        private WriteBatch _writeBatch;
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

        private Dictionary<string, SmartContract> _cacheSmartContracts;
        private Dictionary<string, SmartContract> _cacheTxSmartContracts;
        private Dictionary<string, string> _cacheDataRows;

        internal SmartContracts(IAssemblyHelper assemblyHelper, Networks network)
        {
            _assemblyHelper = assemblyHelper;
            _network = network;
            var options = new Options { CreateIfMissing = true };
            _db = new DB(GetDbFile(), options);
            Init();
            _writeBatch = new WriteBatch();
            _cacheSmartContracts = new Dictionary<string, SmartContract>();
            _cacheDataRows = new Dictionary<string, string>();
            _cacheTxSmartContracts = new Dictionary<string, SmartContract>();
        }

        public void Commit()
        {
            _db.Write(_writeBatch, WriteOptions.Default);
            ClearCache();
        }

        public void Rollback()
        {
            _writeBatch = new WriteBatch();
            ClearCache();
        }

        public bool AddBlock(Block block, bool addInTransaction = false)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var writeBatch = new WriteBatch();
            if (addInTransaction)
            {
                writeBatch = _writeBatch;
            }

            Persist(block, writeBatch, addInTransaction);
            if (!addInTransaction)
            {
                _db.Write(writeBatch, WriteOptions.Default);
            }

            return true;
        }

        public bool AddStorageRow(IEnumerable<byte> scAddr, DataWord key, DataWord value, bool addInTransaction = false)
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
            
            var writeBatch = new WriteBatch();
            if (addInTransaction)
            {
                writeBatch = _writeBatch;
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
                writeBatch.Delete(string.Format(SMART_CONTRACT_STORE_ELT, scAddrHex, keyHex));
            }

            writeBatch.Put(string.Format(SMART_CONTRACT_STORE_ELT, scAddrHex, keyHex), valueHex);
            if (!addInTransaction)
            {
                _db.Write(writeBatch, WriteOptions.Default);
            }
            else
            {
                _cacheDataRows.Add(string.Format(SMART_CONTRACT_STORE_ELT, scAddrHex, keyHex), keyHex);
            }

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
                var cacheSmartContract = _cacheSmartContracts.FirstOrDefault(c => c.Key == smartContractAddress.ToHexString());
                if (cacheSmartContract.Equals(default(KeyValuePair<string, SmartContract>)) || cacheSmartContract.Value == null)
                {
                    return null;
                }

                return cacheSmartContract.Value;
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
                var cacheTxSmartContract = _cacheSmartContracts.FirstOrDefault(c => c.Key == txId.ToHexString());
                if (cacheTxSmartContract.Equals(default(KeyValuePair<string, SmartContract>)) || cacheTxSmartContract.Value == null)
                {
                    return null;
                }

                return cacheTxSmartContract.Value.Address;
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

                var cacheDataRow = _cacheDataRows.FirstOrDefault(c => c.Key == string.Format(SMART_CONTRACT_STORE_ELT, scAddrHex, keyHex));
                if (cacheDataRow.Equals(default(KeyValuePair<string, string>)) || string.IsNullOrWhiteSpace(cacheDataRow.Value))
                {
                    return DataWord.ZERO;
                }

                return new DataWord(cacheDataRow.Value.FromHexString().ToArray());
            }

            return new DataWord(result.FromHexString().ToArray());
        }

        private void Persist(Block block, WriteBatch writeBatch, bool addIntoCache = false)
        {
            if (writeBatch == null)
            {
                throw new ArgumentNullException(nameof(writeBatch));
            }

            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var defaultCallValue = new DataWord(new byte[] { 0x00 });
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
                    while (!program.IsStopped())
                    {
                        solidityVm.Step(program);
                    }

                    var contractCode = program.GetResult().GetHReturn();
                    var txId = transaction.GetTxId();
                    var smartContractAdr = smartContractTransaction.GetSmartContractAddress();
                    var hex = contractCode.ToHexString();
                    writeBatch.Put(string.Format(SMART_CONTRACT_ELT, smartContractAdr.ToHexString()), hex);
                    writeBatch.Put(string.Format(SMART_CONTRACT_TX_ELT, txId.ToHexString()), smartContractAdr.ToHexString());
                    if (addIntoCache)
                    {
                        _cacheSmartContracts.Add(smartContractAdr.ToHexString(), new SmartContract
                        {
                            Address = smartContractAdr,
                            Code = contractCode
                        });
                        _cacheTxSmartContracts.Add(txId.ToHexString(), new SmartContract
                        {
                            Address = smartContractAdr,
                            Code = contractCode
                        });
                    }
                }
                else if (smartContractTransaction.To != null)
                {
                    var sc = GetSmartContract(smartContractTransaction.To);
                    if (sc == null)
                    {
                        return;
                    }

                    var from = smartContractTransaction.From;
                    if (from == null)
                    {
                        from = new byte[0];
                    }

                    var solidityVm = new SolidityVm();
                    var program = new SolidityProgram(sc.Code.ToList(), new SolidityProgramInvoke(smartContractTransaction.Data, smartContractTransaction.To, new DataWord(from.ToArray()), defaultCallValue, this));
                    while (!program.IsStopped())
                    {
                        solidityVm.Step(program);
                    }
                }
            }
        }
        
        private void Persist(SmartContract smartContract, WriteBatch writeBatch, bool addIntoCache = false)
        {
            if (smartContract == null)
            {
                throw new ArgumentNullException(nameof(smartContract));
            }

            if (writeBatch == null)
            {
                throw new ArgumentNullException(nameof(writeBatch));
            }

            writeBatch.Put(string.Format(SMART_CONTRACT_ELT, smartContract.Address.ToHexString()), smartContract.Code.ToHexString());
            if (addIntoCache)
            {
                _cacheSmartContracts.Add(smartContract.Address.ToHexString(), smartContract);
            }
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
            var writeBatch = new WriteBatch();
            foreach(var embeddedContract in _embeddedContracts)
            {
                string result = string.Empty;
                if (!_db.TryGet(string.Format(SMART_CONTRACT_ELT, embeddedContract.Address.ToHexString()), ReadOptions.Default, out result))
                {
                    Persist(embeddedContract, writeBatch);
                }
            }

            _db.Write(writeBatch, WriteOptions.Default);
        }

        private void ClearCache()
        {
            _cacheSmartContracts = new Dictionary<string, SmartContract>();
            _cacheDataRows = new Dictionary<string, string>();
            _cacheTxSmartContracts = new Dictionary<string, SmartContract>();
        }
    }
}
