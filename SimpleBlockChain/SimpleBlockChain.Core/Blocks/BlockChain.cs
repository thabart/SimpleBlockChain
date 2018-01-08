using LevelDB;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleBlockChain.Core.Blocks
{
    public class BlockChain : IDisposable
    {
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly Networks _network;
        private const string _databaseFile = "db_{0}.dat";
        private DB _db;
        private int _currentBlockHeight = 0;
        private IEnumerable<byte> _currentBlockHash = null;

        private const string BLOCK_KEY = "BLOCK_{0}";
        private const string BLOCK_HASH = "BLOCK_HASH_{0}";
        private const string BLOCK_HEIGHT = "BLOCK_HEIGHT_{0}";
        private const string BLOCK_TRANSACTIONS = "BLOCK_TRANSACTIONS_{0}";

        private const string CURRENT_BLOCK_HEIGHT = "CURRENT_BLOCK_HEIGHT";
        private const string CURRENT_BLOCK = "CURRENT_BLOCK";

        private const string TRANSACTION = "TRANSACTION";
        private const string TRANSACTION_ELT = TRANSACTION + "_{0}";
        private const string TRANSACTION_CB = "TRANSACTION_CB";
        private const string TRANSACTION_CB_ELT = TRANSACTION_CB + "_{0}";
        private const string SMART_CONTRACT_TRANSACTION = "SMART_CONTRACT_TRANSACTION";
        private const string SMART_CONTRACT_TRANSACTION_ELT = SMART_CONTRACT_TRANSACTION + "_{0}";

        private const string TX_OUT_UNSPENT = "TX_OUT_UNSPENT";
        private const string TX_OUT_UNSPENT_ELT = TX_OUT_UNSPENT + "_{0}";

        private const char TXID_SEPARATOR = ',';

        internal BlockChain(IAssemblyHelper assemblyHelper, Networks network)
        {
            _assemblyHelper = assemblyHelper;
            _network = network;
            var options = new Options { CreateIfMissing = true };
            _db = new DB(GetDbFile(), options);
            string result = null;
            if (!_db.TryGet(CURRENT_BLOCK, ReadOptions.Default, out result))
            {
                IntitializeBlockChain();
                return;
            }

            IntitializeCurrentBlock();
        }

        public int GetCurrentBlockHeight()
        {
            return _currentBlockHeight;
        }

        public Block GetCurrentBlock()
        {
            return GetBlock(_currentBlockHash);
        }

        public IEnumerable<Block> GetLastBlocks(int nbBlocks)
        {
            if (nbBlocks > _currentBlockHeight)
            {
                nbBlocks = _currentBlockHeight;
            }

            var endIndex = (_currentBlockHeight - nbBlocks) + 1;
            if (endIndex <= 0)
            {
                endIndex = 1;
            }

            var lst = new List<Block>();
            for (var i = _currentBlockHeight; i >= endIndex; i--)
            {
                var block = GetBlock(i);
                if (block == null)
                {
                    continue;
                }

                lst.Add(block);
            }

            return lst;
        }

        public Block GetBlock(int blockHeight)
        {
            string result = null;
            if (!_db.TryGet(string.Format(BLOCK_HASH, blockHeight), ReadOptions.Default, out result))
            {
                return null;
            }

            var hash = result;
            return GetBlock(Convert.FromBase64String(hash));
        }

        public int GetBlockHeight(IEnumerable<byte> hash)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            string result = null;
            if (!_db.TryGet(string.Format(BLOCK_HEIGHT, Convert.ToBase64String(hash.ToArray())), ReadOptions.Default, out result))
            {
                return -1;
            }

            return int.Parse(result);
        }

        public Block GetBlock(IEnumerable<byte> hash)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            var utf8Str = Convert.ToBase64String(hash.ToArray());
            string utf8BlockHeader = string.Empty;
            string utf8BlockTxIds = string.Empty;
            if (!_db.TryGet(string.Format(BLOCK_KEY, utf8Str), ReadOptions.Default, out utf8BlockHeader))
            {
                return null;
            }

            if (!_db.TryGet(string.Format(BLOCK_TRANSACTIONS, utf8Str), ReadOptions.Default, out utf8BlockTxIds))
            {
                return null;
            }
            
            var txIdsPayload = utf8BlockTxIds.Split(TXID_SEPARATOR);
            var transactions = new List<BcBaseTransaction>();
            foreach(var txIdPayload in txIdsPayload)
            {
                string utf8Transaction = string.Empty;
                if (_db.TryGet(string.Format(TRANSACTION_ELT, txIdPayload), ReadOptions.Default, out utf8Transaction))
                {
                    var kvp = BcBaseTransaction.Deserialize(Convert.FromBase64String(utf8Transaction), TransactionTypes.NoneCoinbase);
                    transactions.Add(kvp.Key);
                    continue;
                }

                if (_db.TryGet(string.Format(TRANSACTION_CB_ELT, txIdPayload), ReadOptions.Default, out utf8Transaction))
                {
                    var kvp = BcBaseTransaction.Deserialize(Convert.FromBase64String(utf8Transaction), TransactionTypes.Coinbase);
                    transactions.Add(kvp.Key);
                    continue;
                }
            }

            var blockHeader = Block.DeserializeBlockHeader(Convert.FromBase64String(utf8BlockHeader));
            return new Block(blockHeader, transactions);
        }

        public bool IsUnspent(IEnumerable<byte> txId, uint index)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            Slice result = string.Empty;
            var base64TxId = Convert.ToBase64String(txId.ToArray());
            if (_db.TryGet(ReadOptions.Default, string.Format(TX_OUT_UNSPENT_ELT, string.Format("{0}_{1}", base64TxId, index)), out result))
            {
                return true;
            }

            return false;
        }

        public BlockChain AddBlock(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            _currentBlockHeight++;
            Persist(block);
            return this;
        }

        public bool ContainsUnspentTransaction(IEnumerable<byte> txId, uint index)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }


            var transaction = GetUnspentTransaction(txId, index);
            return transaction != null;
        }

        public IEnumerable<UTXO> GetUnspentTransactions()
        {
            var dic = _db.Find(ReadOptions.Default, TX_OUT_UNSPENT);
            if (dic == null)
            {
                return null;
            }

            return ExtractUnspentTransactions(dic);
        }

        public IEnumerable<UTXO> GetUnspentTransaction(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            var b64TxId = Convert.ToBase64String(txId.ToArray());
            var dic = _db.Find(ReadOptions.Default, string.Format(TX_OUT_UNSPENT_ELT, b64TxId));
            return ExtractUnspentTransactions(dic);
        }

        public UTXO GetUnspentTransaction(IEnumerable<byte> txId, uint index)
        {
            var unspentTransaction = GetUnspentTransaction(txId);
            if (unspentTransaction == null || !unspentTransaction.Any(t => t.Index == index))
            {
                return null;
            }

            return unspentTransaction.First(t => t.Index == index);
        }

        public bool ContainsBlock(IEnumerable<byte> hash)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            return GetBlock(hash) != null;
        }

        public bool ContainsTransaction(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            return GetTransaction(txId) != null;
        }

        public BcBaseTransaction GetTransaction(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            var callback = new Func<bool, BcBaseTransaction>((isCoinBased) =>
            {
                var k = isCoinBased ? TRANSACTION_CB_ELT : TRANSACTION_ELT;
                string result;
                if (!_db.TryGet(string.Format(k, Convert.ToBase64String(txId.ToArray())), ReadOptions.Default, out result))
                {
                    return null;
                }

                var payload = Convert.FromBase64String(result);
                var kvp = BcBaseTransaction.Deserialize(payload, isCoinBased ? TransactionTypes.Coinbase : TransactionTypes.NoneCoinbase);
                if (kvp.Equals(default(KeyValuePair<BcBaseTransaction, int>)) && kvp.Key != null)
                {
                    return null;
                }

                return kvp.Key;
            });

            var res = callback(false);
            if (res == null)
            {
                res = callback(true);
            }

            return res;
        }

        public TransactionOut GetTransactionOut(IEnumerable<byte> txId, int index)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var transaction = GetTransaction(txId);
            if (transaction == null)
            {
                return null;
            }

            return transaction.TransactionOut.ElementAtOrDefault(index);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        private void Persist(Block block)
        {
            _currentBlockHash = block.GetHashHeader();
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var batch = new WriteBatch();
            var hashHeader = Convert.ToBase64String(block.GetHashHeader());
            batch.Put(string.Format(BLOCK_KEY, hashHeader), Convert.ToBase64String(block.SerializeHeader()));
            var allTxIds = new List<IEnumerable<byte>>();
            var allOutpoint = block.Transactions.Where(t => t is NoneCoinbaseTransaction).Select(t => (t.TransactionIn.First() as TransactionInNoneCoinbase).Outpoint);
            foreach (var transaction in block.Transactions)
            {
                var currentTxId = transaction.GetTxId();
                var base64TxId = Convert.ToBase64String(currentTxId.ToArray());
                var cb = transaction as CoinbaseTransaction;
                var arr = transaction.Serialize().ToArray();
                if (cb != null)
                {
                    batch.Put(string.Format(TRANSACTION_CB_ELT, base64TxId), Convert.ToBase64String(arr));
                }
                else
                {
                    batch.Put(string.Format(TRANSACTION_ELT, base64TxId), Convert.ToBase64String(arr));
                }

                if (transaction.TransactionOut != null)
                {
                    foreach(var transactionOut in transaction.TransactionOut)
                    {
                        var index = transaction.TransactionOut.IndexOf(transactionOut);
                        if (!allOutpoint.Any(o => o.Hash.SequenceEqual(transaction.GetTxId()) && o.Index == index))
                        {
                            batch.Put(string.Format(TX_OUT_UNSPENT_ELT, string.Format("{0}_{1}", base64TxId, index)), "empty");
                        }
                    }
                }

                if (transaction.TransactionIn != null)
                {
                    foreach (var transactionIn in transaction.TransactionIn)
                    {
                        var tr = transactionIn as TransactionInNoneCoinbase;
                        if (tr == null)
                        {
                            continue;
                        }

                        var txId = tr.Outpoint.Hash.ToArray();
                        var index = tr.Outpoint.Index;
                        if (IsUnspent(txId, index))
                        {
                            batch.Delete(string.Format(TX_OUT_UNSPENT_ELT, string.Format("{0}_{1}", Convert.ToBase64String(txId), index)));
                        }
                    }
                }

                allTxIds.Add(currentTxId);
            }

            var b64Lst = new List<string>();
            var blockTransactions = new List<byte>();
            foreach (var txId in allTxIds)
            {
                b64Lst.Add(Convert.ToBase64String(txId.ToArray()));
            }

            var b64 = string.Join(TXID_SEPARATOR.ToString(), b64Lst);
            batch.Put(string.Format(BLOCK_TRANSACTIONS, hashHeader), b64);
            batch.Put(CURRENT_BLOCK, Convert.ToBase64String(block.GetHashHeader()));
            batch.Put(CURRENT_BLOCK_HEIGHT, _currentBlockHeight.ToString());
            batch.Put(string.Format(BLOCK_HEIGHT, Convert.ToBase64String(block.GetHashHeader())), _currentBlockHeight.ToString());
            batch.Put(string.Format(BLOCK_HASH, _currentBlockHeight.ToString()), Convert.ToBase64String(block.GetHashHeader()));
            _db.Write(batch, WriteOptions.Default);
        }

        private void IntitializeBlockChain()
        {
            var genesisBlock = Block.BuildGenesisBlock();
            Persist(genesisBlock);
        }

        private void IntitializeCurrentBlock()
        {
            _currentBlockHash = Convert.FromBase64String(_db.Get(CURRENT_BLOCK, ReadOptions.Default));
            var h = _db.Get(CURRENT_BLOCK_HEIGHT, ReadOptions.Default);
            _currentBlockHeight = int.Parse(_db.Get(CURRENT_BLOCK_HEIGHT, ReadOptions.Default));
        }

        private string GetDbFile()
        {
            var path = Path.GetDirectoryName(_assemblyHelper.GetEntryAssembly().Location);
            return Path.Combine(path, string.Format(_databaseFile, GetDirectoryName(_network)));
        }

        private IEnumerable<UTXO> ExtractUnspentTransactions(Dictionary<string, string> dic)
        {
            if (dic == null)
            {
                throw new ArgumentNullException(nameof(dic));
            }

            var result = new List<UTXO>();
            foreach (var kvp in dic)
            {
                var values = kvp.Key.Replace(TX_OUT_UNSPENT, "").Split('_');
                var txId = Convert.FromBase64String(values.ElementAt(1));
                var index = int.Parse(values.ElementAt(2));
                var txOut = (TransactionOut)GetTransactionOut(txId, index);
                if (txOut == null)
                {
                    continue;
                }

                result.Add(new UTXO
                {
                    Index = index,
                    TxId = txId,
                    Script = txOut.Script,
                    Value = txOut.Value
                });
            }

            return result;
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
    }
}
