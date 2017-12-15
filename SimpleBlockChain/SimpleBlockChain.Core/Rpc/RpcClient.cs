using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Rpc.Parameters;
using SimpleBlockChain.Core.Rpc.Responses;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Rpc
{
    public interface IRpcClient
    {
        Task<string> GetRawMemPool();
        Task<BlockTemplate> GetBlockTemplate();
        Task<bool> SubmitBlock(Block block);
        Task<bool> SendRawTransaction(BaseTransaction transaction);
        Task<long> GetUnconfirmedBalance();
        Task<Block> GetBlock(IEnumerable<byte> hash);
        Task<int> GetBlockCount();
        Task<IEnumerable<byte>> GetBlockHash(int height);
        Task<BaseTransaction> GetRawTransaction(IEnumerable<byte> txId);
    }

    public class RpcClient : IRpcClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ContentType = "application/json-rpc";
        private Networks _network;

        public RpcClient(Networks network)
        {
            _httpClientFactory = new HttpClientFactory();
            _network = network;
        }
        
        public async Task<string> GetRawMemPool()
        {
            var httpClient = _httpClientFactory.BuildClient();
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.Getrawmempool);
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, ContentType);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = GetUri()
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<BlockTemplate> GetBlockTemplate()
        {
            var httpClient = _httpClientFactory.BuildClient();
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.Getblocktemplate);
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, ContentType);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = GetUri()
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string errorCode = null;
            var jsonObj = JObject.Parse(json);
            if (TryGetError(jsonObj, out errorCode))
            {
                throw new RpcException(errorCode);
            }

            var resultObj = jsonObj.GetValue("result") as JObject;
            if (resultObj == null)
            {
                return null;
            }

            return BlockTemplate.Deserialize(resultObj);
        }

        public async Task<bool> SubmitBlock(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var parameters = new JArray();
            parameters.Add(block.Serialize().ToHexString());
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.Submitblock);
            jObj.Add("params", parameters);
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, ContentType);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = GetUri()
            };
            
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string errorCode = null;
            var jsonObj = JObject.Parse(json);
            if (TryGetError(jsonObj, out errorCode))
            {
                throw new RpcException(errorCode);
            }

            return true;
        }

        public async Task<bool> SendRawTransaction(BaseTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var parameters = new JArray();
            parameters.Add(transaction.Serialize().ToHexString());
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.SendRawTransaction);
            jObj.Add("params", parameters);
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, ContentType);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = GetUri()
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string errorCode = null;
            var jsonObj = JObject.Parse(json);
            if (TryGetError(jsonObj, out errorCode))
            {
                throw new RpcException(errorCode);
            }

            return true;
        }

        public async Task<IEnumerable<UnspentTransaction>> GetUnspentTransactions(GetUnspentTransactionsParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }


            var httpClient = _httpClientFactory.BuildClient();
            var jAddr = new JArray();
            if (parameter.Addrs != null)
            {
                foreach (var addr in parameter.Addrs)
                {
                    jAddr.Add(addr);
                }
            }

            var parameters = new JArray();
            parameters.Add(parameter.ConfirmationScore);
            parameters.Add(parameter.MaxConfirmations);
            parameters.Add(jAddr);
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.ListUnspent);
            jObj.Add("params", parameters);
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, ContentType);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = GetUri()
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string errorCode = null;
            var jsonObj = JObject.Parse(json);
            if (TryGetError(jsonObj, out errorCode))
            {
                throw new RpcException(errorCode);
            }

            JToken resultToken = null;
            if (!jsonObj.TryGetValue("result", out resultToken) && !(resultToken is JArray))
            {
                throw new RpcException(ErrorCodes.NoResult);
            }

            var arrResult = resultToken as JArray;
            var result = new List<UnspentTransaction>();
            foreach(var res in arrResult)
            {
                var o = res as JObject;
                if (o == null)
                {
                    continue;
                }

                int amount = 0,
                    vout = 0;
                var spendable = false;
                JToken amountToken,
                    voutToken,
                    spendableToken;
                if (o.TryGetValue("amount", out amountToken))
                {
                    if (int.TryParse(amountToken.ToString(), out amount)) { }
                }

                if (o.TryGetValue("vout", out voutToken))
                {
                    if (int.TryParse(voutToken.ToString(), out vout)) { }
                }

                if (o.TryGetValue("spendable", out spendableToken))
                {
                    if (bool.TryParse(spendableToken.ToString(), out spendable)) { }
                }

                result.Add(new UnspentTransaction
                {
                    Address = o.Value<string>("address"),
                    Amount = amount,
                    ScriptPubKey = o.Value<string>("scriptPubKey"),
                    Spendable = spendable,
                    TxId = o.Value<string>("txid"),
                    Vout = vout
                });
            }

            return result;
        }

        public async Task<long> GetUnconfirmedBalance()
        {
            var httpClient = _httpClientFactory.BuildClient();
            var parameters = new JArray();
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.GetUnconfirmedBalance);
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, ContentType);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = GetUri()
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string errorCode = null;
            var jsonObj = JObject.Parse(json);
            if (TryGetError(jsonObj, out errorCode))
            {
                throw new RpcException(errorCode);
            }

            return long.Parse(jsonObj.Value<string>("result"));
        }

        public async Task<Block> GetBlock(IEnumerable<byte> hash)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var jParams = new JArray();
            jParams.Add(hash.ToHexString());
            jParams.Add(0);
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.GetBlock);
            jObj.Add("params", jParams);
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, ContentType);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = GetUri()
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string errorCode = null;
            var jsonObj = JObject.Parse(json);
            if (TryGetError(jsonObj, out errorCode))
            {
                throw new RpcException(errorCode);
            }

            var r = jsonObj.Value<string>("result");
            if (string.IsNullOrWhiteSpace(r))
            {
                return null;
            }

            var payload = r.FromHexString();
            return Block.Deserialize(payload);
        }

        public async Task<int> GetBlockCount()
        {
            var httpClient = _httpClientFactory.BuildClient();
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.GetBlockCount);
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, ContentType);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = GetUri()
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string errorCode = null;
            var jsonObj = JObject.Parse(json);
            if (TryGetError(jsonObj, out errorCode))
            {
                throw new RpcException(errorCode);
            }

            return int.Parse(jsonObj.Value<string>("result"));
        }

        public async Task<IEnumerable<byte>> GetBlockHash(int height)
        {
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var jParams = new JArray();
            jParams.Add(height);
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.GetBlockHash);
            jObj.Add("params", jParams);
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, ContentType);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = GetUri()
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string errorCode = null;
            var jsonObj = JObject.Parse(json);
            if (TryGetError(jsonObj, out errorCode))
            {
                throw new RpcException(errorCode);
            }

            var r = jsonObj.Value<string>("result");
            if (string.IsNullOrWhiteSpace(r))
            {
                return null;
            }

            var payload = r.FromHexString();
            return payload;
        }

        public async Task<BaseTransaction> GetRawTransaction(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentOutOfRangeException(nameof(txId));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var jParams = new JArray();
            jParams.Add(txId.ToHexString());
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.GetRawTransaction);
            jObj.Add("params", jParams);
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, ContentType);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = GetUri()
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            string errorCode = null;
            var jsonObj = JObject.Parse(json);
            if (TryGetError(jsonObj, out errorCode))
            {
                throw new RpcException(errorCode);
            }

            var r = jsonObj.Value<string>("result");
            if (string.IsNullOrWhiteSpace(r))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(r))
            {
                return null;
            }

            var payload = r.FromHexString();
            BaseTransaction transaction = null;
            try
            {
                transaction = BaseTransaction.Deserialize(payload, TransactionTypes.NoneCoinbase).Key;
            }
            catch (Exception) { }
            if (transaction == null)
            {
                try
                {
                    transaction = BaseTransaction.Deserialize(payload, TransactionTypes.Coinbase).Key;
                }
                catch (Exception) { }
            }

            return transaction;
        }

        public static bool TryGetError(JObject jObj, out string errorCode)
        {
            if (jObj == null)
            {
                throw new ArgumentNullException(nameof(jObj));
            }

            errorCode = null;
            JToken errorToken = null;
            if (!jObj.TryGetValue("error", out errorToken))
            {
                return false;
            }

            errorCode = errorToken.ToString();
            return true;
        }

        private Uri GetUri()
        {
            return new Uri($"http://localhost:{PortsHelper.GetRPCPort(_network)}");
        }
    }
}
