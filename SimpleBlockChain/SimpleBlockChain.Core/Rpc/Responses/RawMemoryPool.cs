namespace SimpleBlockChain.Core.Rpc.Responses
{
    public class RawMemoryPool
    {
        public string TxId { get; set; }
        public long Fee { get; set; }
        public long ModifiedFee { get; set; }
        public long Time { get; set; }
    }
}
