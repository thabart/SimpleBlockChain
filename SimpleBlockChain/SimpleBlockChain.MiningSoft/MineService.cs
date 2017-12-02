using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace SimpleBlockChain.MiningSoft
{
    public class MineService : IDisposable
    {
        private readonly AutoResetEvent _autoEvent = null;
        private readonly RpcClient _rpcClient;
        private readonly Semaphore _pool;
        public const int DEFAULT_MINE_INTERVAL = 10000;
        private Timer _timer;

        public MineService(Networks network)
        {
            _autoEvent = new AutoResetEvent(false);
            _rpcClient = new RpcClient(network);
            _pool = new Semaphore(0, 1);
        }

        public event EventHandler<EventArgs> StartMiningEvent;
        public event EventHandler<EventArgs> EndMiningEvent;

        public void Start()
        {
            Mine(null);
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        private void Mine(object sender)
        {
            try
            {
                var blockTemplate = _rpcClient.GetBlockTemplate().Result;
                var block = CalculateHeader(blockTemplate, 0, 0);
                if (block == null)
                {
                    Mine(null);
                }

                // SUBMIT THE BLOCK.

                _timer = new Timer(Mine, _autoEvent, DEFAULT_MINE_INTERVAL, DEFAULT_MINE_INTERVAL);
            }
            catch(RpcException rpcException)
            {
                string s = "";
            }
        }

        private static Block CalculateHeader(BlockTemplate blockTemplate, uint nonce, uint extraNonce)
        {
            var transactions = new List<BaseTransaction>();
            var coinBaseInTrans = blockTemplate.CoinBaseTx.TransactionIn[0] as TransactionInCoinbase;
            coinBaseInTrans.CoinBaseScript = BitConverter.GetBytes(extraNonce);
            transactions.Add(blockTemplate.CoinBaseTx);
            transactions.AddRange(blockTemplate.Transactions);
            var block = new Block(blockTemplate.PreviousBlockHash, blockTemplate.Bits, nonce, blockTemplate.Version);
            block.Transactions = transactions;
            var serialized = block.GetHashHeader();
            if (TargetHelper.IsValid(serialized, blockTemplate.Target))
            {
                return block;
            }

            var difference = DateTime.UtcNow.ToUnixTimeUInt32() - blockTemplate.CurrentTime;
            if (difference >= blockTemplate.Expires) // EXPIRATION.
            {
                return null;
            }

            if (nonce == uint.MaxValue)
            {
                nonce = 0;
                extraNonce++;
            }

            Thread.Sleep(100);
            nonce++;
            return CalculateHeader(blockTemplate, nonce, extraNonce);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
