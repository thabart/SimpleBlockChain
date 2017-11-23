using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.LevelDb;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Blocks
{
    public class BlockChain : IDisposable
    {
        private const string _databaseFile = "db.dat";
        private DB _db;
        private int _currentBlockHeight = 0;
        private IEnumerable<byte> _currentBlockHash = null;
        private const string BLOCK_KEY = "BLOCK_{0}";
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
            _db = new DB(new Options { CreateIfMissing = true }, _databaseFile);
            string result = null;
            if (!_db.TryGet(null, CURRENT_BLOCK, out result))
            {
                IntitializeBlockChain();
                return;
            }

            IntitializeCurrentBlock();
        }

        public Block GetCurrentBlock()
        {
            return GetBlock(_currentBlockHash);
        }
        
        public Block GetBlock(IEnumerable<byte> hash)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            var utf8Str = System.Text.Encoding.UTF8.GetString(hash.ToArray());
            string utf8BlockHeader = null;
            string utf8BlockTxIds = null;
            if (!_db.TryGet(null, string.Format(BLOCK_KEY, utf8Str), out utf8BlockHeader))
            {
                return null;
            }

            if (!_db.TryGet(null, string.Format(BLOCK_TRANSACTIONS, utf8Str), out utf8BlockTxIds))
            {
                return null;
            }

            var txIdsPayload = utf8BlockTxIds.Split(TXID_SEPARATOR);
            var transactions = new List<BaseTransaction>();
            foreach(var txIdPayload in txIdsPayload)
            {
                string utf8Transaction = null;
                if (_db.TryGet(null, string.Format(SPENT_TRANSACTION, txIdPayload), out utf8Transaction))
                {
                    var kvp = BaseTransaction.Deserialize(System.Text.Encoding.UTF8.GetBytes(utf8Transaction), TransactionTypes.NoneCoinbase);
                    transactions.Add(kvp.Key);
                    continue;
                }

                if (_db.TryGet(null, string.Format(UNSPENT_TRANSACTION, txIdPayload), out utf8Transaction))
                {
                    var kvp = BaseTransaction.Deserialize(System.Text.Encoding.UTF8.GetBytes(utf8Transaction), TransactionTypes.NoneCoinbase);
                    transactions.Add(kvp.Key);
                    continue;
                }

                if (_db.TryGet(null, string.Format(SPENT_TRANSACTION_CB, txIdPayload), out utf8Transaction))
                {
                    var kvp = BaseTransaction.Deserialize(System.Text.Encoding.UTF8.GetBytes(utf8Transaction), TransactionTypes.Coinbase);
                    transactions.Add(kvp.Key);
                    continue;
                }

                if (_db.TryGet(null, string.Format(UNSPENT_TRANSACTION_CB, txIdPayload), out utf8Transaction))
                {
                    var kvp = BaseTransaction.Deserialize(System.Text.Encoding.UTF8.GetBytes(utf8Transaction), TransactionTypes.Coinbase);
                    transactions.Add(kvp.Key);
                    continue;
                }
            }

            var blockHeader = Block.DeserializeBlockHeader(System.Text.Encoding.UTF8.GetBytes(utf8BlockHeader));
            return new Block(blockHeader, transactions);
        }

        public BlockChain AddBlock(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            /*
            var lastBlock = Blocks.Last();
            var newPreviousBlockHashHeader = lastBlock.GetHashHeader();
            if (!block.BlockHeader.PreviousBlockHeader.SequenceEqual(newPreviousBlockHashHeader))
            {
                throw new ValidationException(ErrorCodes.PreviousHashBlockDoesntMatch);
            }
            */

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

            var result = _db.Get(null, string.Format(UNSPENT_TRANSACTION, System.Text.Encoding.UTF8.GetString(txId.ToArray())));
            if (string.IsNullOrWhiteSpace(result))
            {
                return null;
            }

            var payload = System.Text.Encoding.UTF8.GetBytes(result);
            var kvp = BaseTransaction.Deserialize(payload, TransactionTypes.NoneCoinbase);
            if (kvp.Equals(default(KeyValuePair<BaseTransaction, int>)) && kvp.Key != null)
            {
                return null;
            }
            
            if (kvp.Key.TransactionOut.Count() >= index)
            {
                return null;
            }

            return kvp.Key;
        }

        public bool ContainsTransaction(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            return false;
        }

        public BaseTransaction GetTransaction(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }

            return null;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        private void Persist(Block block)
        {
            _currentBlockHeight++;
            _currentBlockHash = block.SerializeHeader();
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var batch = new WriteBatch();
            var hashHeader = System.Text.ASCIIEncoding.UTF8.GetString(block.SerializeHeader());
            batch.Put(string.Format(BLOCK_KEY, hashHeader), System.Text.Encoding.UTF8.GetString(block.SerializeHeader()));
            var allTxIds = new List<IEnumerable<byte>>();
            foreach (var transaction in block.Transactions)
            {
                var currentTxId = transaction.GetTxId();
                var cb = transaction as CoinbaseTransaction;
                if (cb != null)
                {
                    batch.Put(string.Format(UNSPENT_TRANSACTION, System.Text.Encoding.UTF8.GetString(currentTxId.ToArray())), System.Text.Encoding.UTF8.GetString(transaction.Serialize().ToArray()));
                }
                else
                {
                    batch.Put(string.Format(UNSPENT_TRANSACTION_CB, System.Text.Encoding.UTF8.GetString(currentTxId.ToArray())), System.Text.Encoding.UTF8.GetString(transaction.Serialize().ToArray()));
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
                        batch.Delete(string.Format(isNoneCoinBaseTransaction ? UNSPENT_TRANSACTION : UNSPENT_TRANSACTION_CB, System.Text.Encoding.UTF8.GetString(txId)));
                        batch.Put(string.Format(isNoneCoinBaseTransaction ? SPENT_TRANSACTION : SPENT_TRANSACTION_CB, System.Text.Encoding.UTF8.GetString(txId)), System.Text.Encoding.UTF8.GetString(unspentTransaction.Serialize().ToArray()));
                    }
                }

                allTxIds.Add(currentTxId);
            }

            var blockTransactions = new List<byte>();
            foreach (var txId in allTxIds)
            {
                blockTransactions.AddRange(txId);
                blockTransactions.AddRange(System.Text.Encoding.UTF8.GetBytes(new char[] { TXID_SEPARATOR }));
            }

            batch.Put(string.Format(BLOCK_TRANSACTIONS, hashHeader), System.Text.Encoding.UTF8.GetString(blockTransactions.ToArray()));
            batch.Put(CURRENT_BLOCK, System.Text.Encoding.UTF8.GetString(block.GetHashHeader()));
            batch.Put(CURRENT_BLOCK_HEIGHT, _currentBlockHeight);

        }

        private void IntitializeBlockChain()
        {
            var genesisBlock = Block.BuildGenesisBlock();
            Persist(genesisBlock);
        }

        private void IntitializeCurrentBlock()
        {
            _currentBlockHash = System.Text.Encoding.UTF8.GetBytes(_db.Get(null, CURRENT_BLOCK));
            _currentBlockHeight = int.Parse(_db.Get(null, CURRENT_BLOCK_HEIGHT));
        }
    }
}
