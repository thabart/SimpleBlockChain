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

        private const string SMART_CONTRACT = "SMART_CONTRACT";
        private const string SMART_CONTRACT_ELT = SMART_CONTRACT + "_{0}";

        private const string SMART_CONTRACT_TX = "SMART_CONTRACT_TX";
        private const string SMART_CONTRACT_TX_ELT = SMART_CONTRACT_TX + "_{0}";

        internal SmartContracts(IAssemblyHelper assemblyHelper, Networks network)
        {
            _assemblyHelper = assemblyHelper;
            _network = network;
            var options = new Options { CreateIfMissing = true };
            _db = new DB(GetDbFile(), options);
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
            if (_db.TryGet(string.Format(SMART_CONTRACT_ELT, smartContractAddress.ToHexString()), ReadOptions.Default, out result))
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
            if (_db.TryGet(string.Format(SMART_CONTRACT_TX_ELT, txId.ToHexString()), ReadOptions.Default, out result))
            {
                return null;
            }

            return result.FromHexString();
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

                if (smartContractTransaction.To == null)
                {
                    var solidityVm = new SolidityVm();
                    var program = new SolidityProgram(smartContractTransaction.Data.ToList(), new SolidityProgramInvoke(new DataWord(smartContractTransaction.From.ToArray()), defaultCallValue));
                    while(!program.IsStopped())
                    {
                        program.Step();
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

        public void Dispose()
        {
            _db.Dispose();
        }

        private string GetDbFile()
        {
            var path = Path.GetDirectoryName(_assemblyHelper.GetEntryAssembly().Location);
            return Path.Combine(path, string.Format(_databaseFile, GetDirectoryName(_network)));
        }

        private static string GetDirectoryName(Networks networkEnum)
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
    }
}
