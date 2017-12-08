using LevelDB;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleBlockChain.Core.Blocks
{
    public class BlockChain : IDisposable
    {
        private const string _databaseFile = "db.dat";
        private DB _db;
        private int _currentBlockHeight = 0;
        private IEnumerable<byte> _currentBlockHash = null;
        private const string BLOCK_KEY = "BLOCK_{0}";
        private const string BLOCK_HASH = "BLOCK_HASH_{0}";
        private const string BLOCK_HEIGHT = "BLOCK_HEIGHT_{0}";
        private const string SPENT_TRANSACTION = "SPEND_TRANSACTION_{0}";
        private const string UNSPENT_TRANSACTION = "UNSPENT_TRANSACTION_{0}";
        private const string BLOCK_TRANSACTIONS = "BLOCK_TRANSACTIONS_{0}";
        private const string UNSPENT_TRANSACTION_CB = "UNSPENT_TRANSACTION_CB_{0}";
        private const string SPENT_TRANSACTION_CB = "SPENT_TRANSACTION_CB_{0}";
        private const string CURRENT_BLOCK_HEIGHT = "CURRENT_BLOCK_HEIGHT";
        private const string CURRENT_BLOCK = "CURRENT_BLOCK";
        private const char TXID_SEPARATOR = ',';

        public BlockChain()
        {
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
            var transactions = new List<BaseTransaction>();
            foreach(var txIdPayload in txIdsPayload)
            {
                string utf8Transaction = string.Empty;
                if (_db.TryGet(string.Format(SPENT_TRANSACTION, txIdPayload), ReadOptions.Default, out utf8Transaction))
                {
                    var kvp = BaseTransaction.Deserialize(Convert.FromBase64String(utf8Transaction), TransactionTypes.NoneCoinbase);
                    transactions.Add(kvp.Key);
                    continue;
                }

                if (_db.TryGet(string.Format(UNSPENT_TRANSACTION, txIdPayload), ReadOptions.Default, out utf8Transaction))
                {
                    var kvp = BaseTransaction.Deserialize(Convert.FromBase64String(utf8Transaction), TransactionTypes.NoneCoinbase);
                    transactions.Add(kvp.Key);
                    continue;
                }

                if (_db.TryGet(string.Format(SPENT_TRANSACTION_CB, txIdPayload), ReadOptions.Default, out utf8Transaction))
                {
                    var kvp = BaseTransaction.Deserialize(Convert.FromBase64String(utf8Transaction), TransactionTypes.Coinbase);
                    transactions.Add(kvp.Key);
                    continue;
                }

                if (_db.TryGet(string.Format(UNSPENT_TRANSACTION_CB, txIdPayload), ReadOptions.Default, out utf8Transaction))
                {
                    var kvp = BaseTransaction.Deserialize(Convert.FromBase64String(utf8Transaction), TransactionTypes.Coinbase);
                    transactions.Add(kvp.Key);
                    continue;
                }
            }

            var blockHeader = Block.DeserializeBlockHeader(Convert.FromBase64String(utf8BlockHeader));
            return new Block(blockHeader, transactions);
        }

        public BlockChain AddBlock(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            block.Check();
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

        public BaseTransaction GetUnspentTransaction(IEnumerable<byte> txId, uint index)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            var callback = new Func<bool, BaseTransaction>((isCoinBased) =>
            {
                var k = isCoinBased ? UNSPENT_TRANSACTION_CB : UNSPENT_TRANSACTION;
                string result;
                if (!_db.TryGet(string.Format(k, Convert.ToBase64String(txId.ToArray())), ReadOptions.Default, out result))
                {
                    return null;
                }

                var payload = Convert.FromBase64String(result);
                var kvp = BaseTransaction.Deserialize(payload, isCoinBased ? TransactionTypes.Coinbase : TransactionTypes.NoneCoinbase);
                if (kvp.Equals(default(KeyValuePair<BaseTransaction, int>)) && kvp.Key != null)
                {
                    return null;
                }

                if (kvp.Key.TransactionOut.Count() <= index)
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

        public BaseTransaction GetTransaction(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            var callback = new Func<bool, BaseTransaction>((isCoinBased) =>
            {
                var k = isCoinBased ? UNSPENT_TRANSACTION_CB : UNSPENT_TRANSACTION;
                string result;
                if (!_db.TryGet(string.Format(k, Convert.ToBase64String(txId.ToArray())), ReadOptions.Default, out result))
                {
                    return null;
                }

                var payload = Convert.FromBase64String(result);
                var kvp = BaseTransaction.Deserialize(payload, isCoinBased ? TransactionTypes.Coinbase : TransactionTypes.NoneCoinbase);
                if (kvp.Equals(default(KeyValuePair<BaseTransaction, int>)) && kvp.Key != null)
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

        public void Dispose()
        {
            _db.Dispose();
        }

        private void Persist(Block block)
        {
            _currentBlockHeight++;
            _currentBlockHash = block.GetHashHeader();
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var batch = new WriteBatch();
            var hashHeader = Convert.ToBase64String(block.GetHashHeader());
            batch.Put(string.Format(BLOCK_KEY, hashHeader), Convert.ToBase64String(block.SerializeHeader()));
            var allTxIds = new List<IEnumerable<byte>>();
            foreach (var transaction in block.Transactions)
            {
                var currentTxId = transaction.GetTxId();
                var cb = transaction as CoinbaseTransaction;
                var arr = transaction.Serialize().ToArray();
                if (cb != null)
                {
                    batch.Put(string.Format(UNSPENT_TRANSACTION_CB, Convert.ToBase64String(currentTxId.ToArray())), Convert.ToBase64String(arr));
                }
                else
                {
                    batch.Put(string.Format(UNSPENT_TRANSACTION, Convert.ToBase64String(currentTxId.ToArray())), Convert.ToBase64String(arr));
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
                        var unspentTransaction = GetUnspentTransaction(txId, index); // Transaction should not be NULL.
                        var isNoneCoinBaseTransaction = unspentTransaction as NoneCoinbaseTransaction != null;
                        batch.Delete(string.Format(isNoneCoinBaseTransaction ? UNSPENT_TRANSACTION : UNSPENT_TRANSACTION_CB, Convert.ToBase64String(txId)));
                        batch.Put(string.Format(isNoneCoinBaseTransaction ? SPENT_TRANSACTION : SPENT_TRANSACTION_CB, Convert.ToBase64String(txId)), Convert.ToBase64String(unspentTransaction.Serialize().ToArray()));
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

        private static string GetDbFile()
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Path.Combine(path, _databaseFile);
        }
    }
}
