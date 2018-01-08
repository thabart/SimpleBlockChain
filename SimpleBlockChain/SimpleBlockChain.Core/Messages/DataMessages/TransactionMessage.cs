﻿using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Messages.DataMessages
{
    public class TransactionMessage : Message
    {
        public TransactionMessage(BcBaseTransaction transaction, Networks network) : base(network)
        {
            Transaction = transaction;
        }

        public BcBaseTransaction Transaction { get; private set; }

        public override string GetCommandName()
        {
            return Constants.MessageNames.Transaction;
        }

        public static TransactionMessage Deserialize(IEnumerable<byte> payload, Networks network, TransactionTypes transactionType)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            return new TransactionMessage(BcBaseTransaction.Deserialize(payload, transactionType).Key, network);
        }

        protected override byte[] GetSerializedContent()
        {
            return Transaction.Serialize().ToArray();
        }
    }
}
