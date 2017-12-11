using System.Collections.Generic;

namespace SimpleBlockChain.Core.Rpc.Parameters
{
    public class GetUnspentTransactionsParameter
    {
        public GetUnspentTransactionsParameter()
        {
            ConfirmationScore = 1;
            MaxConfirmations = 9999999;
            Addrs = new List<string>();
        }

        public int ConfirmationScore { get; set; }
        public int MaxConfirmations { get; set; }
        public IEnumerable<string> Addrs { get; set; }
    }
}
