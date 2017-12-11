namespace SimpleBlockChain.Core.Rpc.Responses
{
    public class UnspentTransaction
    {
        public string TxId { get; set; }
        public int Vout { get; set; }
        public string Address { get; set; }
        public string ScriptPubKey { get; set; }
        public int Amount { get; set; }
        public bool Spendable { get; set; }
    }
}
