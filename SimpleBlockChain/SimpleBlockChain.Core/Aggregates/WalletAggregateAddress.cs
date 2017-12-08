using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Exceptions;
using System;

namespace SimpleBlockChain.Core.Aggregates
{
    public class WalletAggregateAddress
    {
        private const string _hash = "hash";
        private const string _key = "key";
        private const string _network = "network";

        public string Hash { get; set; }
        public Key Key { get; set; }
        public Networks Network { get; set; }

        public JObject GetJson()
        {
            var result = new JObject();
            if (string.IsNullOrWhiteSpace(Hash))
            {
                throw new SerializeException(string.Format(ErrorCodes.ParameterMissing, _hash));
            }

            if (Key == null)
            {
                throw new SerializeException(string.Format(ErrorCodes.ParameterMissing, _hash));
            }

            result.Add(_hash, Hash);
            result.Add(_key, Key.GetJson());
            result.Add(_network, (int)Network);
            return result;
        }

        public static WalletAggregateAddress FromJson(JObject jObj)
        {
            if (jObj == null)
            {
                throw new ArgumentNullException(nameof(jObj));
            }

            JToken hashToken = null;
            if (!jObj.TryGetValue(_hash, out hashToken))
            {
                throw new ParseException(string.Format(ErrorCodes.ParameterMissing, _hash));
            }

            JToken keyToken = null;
            if (!jObj.TryGetValue(_key, out keyToken))
            {
                throw new ParseException(string.Format(ErrorCodes.ParameterMissing, _key));
            }

            JToken networkToken;
            if (!jObj.TryGetValue(_network, out networkToken))
            {
                throw new ParseException(string.Format(ErrorCodes.ParameterMissing, _network));
            }

            int network = 0;
            if (!int.TryParse(networkToken.ToString(), out network))
            {
                throw new ParseException(ErrorCodes.NotCorrectNetwork);
            }

            Networks networkEnum = (Networks)network;
            var hash = hashToken.ToString();
            var keyObj = JObject.Parse(keyToken.ToString());
            if (keyObj == null)
            {
                throw new ParseException(string.Format(ErrorCodes.NotValidJson, _key));
            }

            return new WalletAggregateAddress
            {
                Hash = hash,
                Key = Key.FromJson(keyObj),
                Network = networkEnum
            };
        }
    }
}
