using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Transactions;
using System;

namespace SimpleBlockChain.Core.Evts
{
    internal class P2PConnectorEventStore
    {
        private static P2PConnectorEventStore _instance;

        public static P2PConnectorEventStore Instance()
        {
            if (_instance == null)
            {
                _instance = new P2PConnectorEventStore();
            }

            return _instance;
        }

        public void Broadcast(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            if (NewBlockEvt != null)
            {
                NewBlockEvt(this, new BlockEventArgs(block));
            }
        }

        public void Broadcast(SmartContractTransaction transaction)
        {
            if (transaction ==  null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (NewSmartContractTransactionEvt != null)
            {
                NewSmartContractTransactionEvt(this, new SmartContractTransactionEventArgs(transaction));
            }
        }

        public void Broadcast(BcBaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (NewTransactionEvt != null)
            {
                NewTransactionEvt(this, new TransactionEventArgs(transaction));
            }
        }

        public event EventHandler<BlockEventArgs> NewBlockEvt;
        public event EventHandler<TransactionEventArgs> NewTransactionEvt;
        public event EventHandler<SmartContractTransactionEventArgs> NewSmartContractTransactionEvt;
    }
}
