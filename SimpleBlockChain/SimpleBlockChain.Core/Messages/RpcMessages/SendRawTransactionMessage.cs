using SimpleBlockChain.Core.Transactions;
using System.Linq;

namespace SimpleBlockChain.Core.Messages.RpcMessages
{
    public class SendRawTransactionMessage : Message
    {
        public SendRawTransactionMessage(Transaction transaction, Networks network) : base(network)
        {
            Transaction = transaction;
        }

        public Transaction Transaction { get; private set; }

        public override string GetCommandName()
        {
            return Constants.MessageNames.SendRawTransactionMessage;
        }
        
        public static SendRawTransactionMessage Deserialize(byte[] payload, Networks network)
        {
            return new SendRawTransactionMessage(Transaction.Deserialize(payload), network);
        }

        protected override byte[] GetSerializedContent()
        {
            return Transaction.Serialize().ToArray();
        }
    }
}
