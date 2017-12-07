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

        public string Hash { get; set; }
        public Key Key { get; set; }

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

            var hash = hashToken.ToString();
            var keyObj = JObject.Parse(keyToken.ToString());
            if (keyObj == null)
            {
                throw new ParseException(string.Format(ErrorCodes.NotValidJson, _key));
            }

            return new WalletAggregateAddress
            {
                Hash = hash,
                Key = Key.FromJson(keyObj)
            };
        }
    }
}
