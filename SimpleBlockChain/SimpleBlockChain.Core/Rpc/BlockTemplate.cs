using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SimpleBlockChain.Core.Rpc
{
    public class BlockTemplate
    {
        public BaseTransaction CoinBaseTx { get; set; }
        public IEnumerable<byte> PreviousBlockHash { get; set; }
        public IEnumerable<BaseTransaction> Transactions { get; set; }
        public double Expires { get; set; }
        public int Height { get; set; }
        public string LongPollId { get; set; }
        public int Version { get; set; }
        public uint CurrentTime { get; set; }
        public uint Bits { get; set; }
        public byte[] Target { get; set; }

        public static BlockTemplate Deserialize(JObject jObj)
        {
            if (jObj == null)
            {
                throw new ArgumentNullException(nameof(jObj));
            }

            var result = new BlockTemplate();
            var coinBaseTxn = jObj.SelectToken("coinbasetxn");
            if (coinBaseTxn != null)
            {
                var coinBaseTxnDataObj = coinBaseTxn as JObject;
                if (coinBaseTxnDataObj != null)
                {
                    JToken coinBaseTxnDataToken = null;
                    if (coinBaseTxnDataObj.TryGetValue("data", out coinBaseTxnDataToken))
                    {
                        var data = coinBaseTxnDataToken.ToString().FromHexString();
                        result.CoinBaseTx = BaseTransaction.Deserialize(data, TransactionTypes.Coinbase).Key;
                    }
                }
            }

            JToken previousBlockHashToken = null;
            if (jObj.TryGetValue("previousblockhash", out previousBlockHashToken))
            {
                result.PreviousBlockHash = previousBlockHashToken.ToString().FromHexString();
            }

            JToken transactionsToken = null;
            if (jObj.TryGetValue("transactions", out transactionsToken))
            {
                var transactionsArr = transactionsToken as JArray;
                if (transactionsArr != null)
                {
                    var transactions = new List<BaseTransaction>();
                    foreach(var transactionObj in transactionsArr)
                    {
                        transactions.Add(BaseTransaction.Deserialize(transactionObj.ToString().FromHexString(), TransactionTypes.NoneCoinbase).Key);
                    }

                    result.Transactions = transactions;
                }
            }

            JToken heightToken = null;
            if (jObj.TryGetValue("height", out heightToken))
            {
                result.Height = int.Parse(heightToken.ToString());
            }

            JToken versionToken = null;
            if (jObj.TryGetValue("version", out versionToken))
            {
                result.Version = int.Parse(versionToken.ToString());
            }

            JToken curTimeToken = null;
            if (jObj.TryGetValue("curtime", out curTimeToken))
            {
                result.CurrentTime = uint.Parse(curTimeToken.ToString());
            }

            JToken bitsToken = null;
            if (jObj.TryGetValue("bits", out bitsToken))
            {
                result.Bits = uint.Parse(bitsToken.ToString());
            }

            JToken targetToken = null;
            if (jObj.TryGetValue("target", out targetToken))
            {
                result.Target = targetToken.ToString().FromHexString().ToArray();
            }

            JToken expiresToken = null;
            if (jObj.TryGetValue("expires", out expiresToken))
            {
                result.Expires = double.Parse(expiresToken.ToString());
            }

            return result;
        }
    }
}
