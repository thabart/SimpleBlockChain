using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Transactions
{
    public class TransactionInputBuilder
    {
        private TransactionIn _transactionIn;

        public TransactionInputBuilder()
        {
            _transactionIn = new TransactionIn();
        }

        public TransactionInputBuilder New()
        {
            _transactionIn = new TransactionIn();
            return this;
        }

        public TransactionInputBuilder AddOutput(IEnumerable<byte> hash, uint index)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            _transactionIn.Outpoint = new Outpoint(hash, index);
            return this;
        }

        public TransactionInputBuilder AddSignatureScript(IEnumerable<byte> sig, IEnumerable<byte> publicKey)
        {
            var result = new List<byte>();
            result.AddRange(sig);
            result.AddRange(publicKey);
            _transactionIn.SignatureScript = result;
            return this;
        }

        public TransactionIn Create()
        {
            return _transactionIn;
        }
    }
}
