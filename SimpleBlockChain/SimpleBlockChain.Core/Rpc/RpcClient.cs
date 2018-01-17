using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Rpc.Parameters;
using SimpleBlockChain.Core.Rpc.Responses;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Rpc
{
    public interface IRpcClient
    {
        Task<IEnumerable<RawMemoryPool>> GetRawMemPool(bool verboseOutput = false);
        Task<BlockTemplate> GetBlockTemplate();
        Task<bool> SubmitBlock(Block block);
        Task<string> SendRawTransaction(BaseTransaction transaction, bool allowHighFees = false);
        Task<long> GetUnconfirmedBalance();
        Task<Block> GetBlock(IEnumerable<byte> hash);
        Task<int> GetBlockCount();
        Task<IEnumerable<byte>> GetBlockHash(int height);
        Task<BaseTransaction> GetRawTransaction(IEnumerable<byte> txId);
        Task<string> CallSmartContract(SmartContractTransactionParameter scTransaction);
        Task<CompileSolidityResponse> CompileSolidity(string contract);
        Task<TransactionReceiptResponse> GetTransactionReceipt(IEnumerable<byte> txId);
        Task<IEnumerable<byte>> AddFilter(IEnumerable<byte> scAddr);
        Task<IEnumerable<GetFilterChangeResponse>> GetFilterChanges(IEnumerable<byte> filterId);
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
        
        public async Task<IEnumerable<RawMemoryPool>> GetRawMemPool(bool verboseOutput = false)
        {
            var httpClient = _httpClientFactory.BuildClient();
            var jParams = new JArray();
            jParams.Add(verboseOutput ? 0 : 1);
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.Getrawmempool);
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
            var jsonObj = JObject.Parse(json);
            string errorCode = null;
            if (TryGetError(jsonObj, out errorCode))
            {
                throw new RpcException(errorCode);
            }

            var resultObj = jsonObj.GetValue("result") as JArray;
            return ExtractRawMemPool(resultObj);
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

        public async Task<string> SendRawTransaction(BaseTransaction transaction, bool allowHighFees = false)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var parameters = new JArray();
            parameters.Add(transaction.Serialize().ToHexString());
            parameters.Add(allowHighFees);
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

            return jsonObj.GetValue("result").ToString();
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
                    Confirmations = int.Parse(o.Value<string>("confirmations")),
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
                transaction = BaseTransaction.Deserialize(payload).Key;
            }
            catch (Exception) { }
            return transaction;
        }

        public async Task<string> CallSmartContract(SmartContractTransactionParameter scTransaction)
        {
            if (scTransaction == null)
            {
                throw new ArgumentNullException(nameof(scTransaction));
            }

            if (scTransaction.To == null)
            {
                throw new ArgumentNullException(nameof(scTransaction.To));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var jParams = new JArray();
            jParams.Add(scTransaction.To.ToHexString());
            jParams.Add(scTransaction.From.ToHexString());
            jParams.Add(scTransaction.Data.ToHexString());
            jParams.Add(scTransaction.Gas);
            jParams.Add(scTransaction.GasPrice);
            jParams.Add(scTransaction.Value);
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.ScCall);
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

            return r;
        }

        public async Task<CompileSolidityResponse> CompileSolidity(string contract)
        {
            if (string.IsNullOrWhiteSpace(contract))
            {
                throw new ArgumentNullException(nameof(contract));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var jParams = new JArray();
            jParams.Add(contract);
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.CompileSolidity);
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

            var r = jsonObj.GetValue("result").ToString();
            if (string.IsNullOrWhiteSpace(r))
            {
                return null;
            }

            var resultObj = JObject.Parse(r);
            var result = new CompileSolidityResponse
            {
                CompilerVersion = resultObj.GetValue("compilerVersion").ToString(),
                Language = resultObj.GetValue("language").ToString(),
                LanguageVersion = resultObj.GetValue("languageVersion").ToString(),
                Source = resultObj.Value<string>("source")
            };
            var infoLst = JArray.Parse(resultObj.GetValue("info").ToString());
            var lstInfo = new List<CompileSolidityResponseInfo>();
            foreach(JToken info in infoLst)
            {
                var infoObj = info as JObject;
                if (infoObj == null)
                {
                    continue;
                }

                lstInfo.Add(new CompileSolidityResponseInfo
                {
                    AbiDefinition = JArray.Parse(infoObj.GetValue("abiDefinition").ToString()),
                    Code = infoObj.GetValue("code").ToString()
                });
            }

            result.Infos = lstInfo;
            return result;
        }

        public async Task<TransactionReceiptResponse> GetTransactionReceipt(IEnumerable<byte> txId)
        {
            if (txId == null)
            {
                throw new ArgumentNullException(nameof(txId));
            }


            var httpClient = _httpClientFactory.BuildClient();
            var jParams = new JArray();
            jParams.Add(txId.ToHexString());
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.GetTransactionReceipt);
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

            var r = jsonObj.GetValue("result").ToString();
            if (string.IsNullOrWhiteSpace(r))
            {
                return null;
            }

            var resultObj = JObject.Parse(r);
            var result = new TransactionReceiptResponse
            {
                TransactionHash = resultObj.GetValue("transactionHash").ToString(),
                ContractAddress= resultObj.GetValue("contractAddress").ToString()
            };
            return result;
        }

        public async Task<IEnumerable<byte>> AddFilter(IEnumerable<byte> scAddr)
        {
            if (scAddr == null)
            {
                throw new ArgumentNullException(nameof(scAddr));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var jParams = new JArray();
            jParams.Add(scAddr.ToHexString());
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.NewFilter);
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

            var r = jsonObj.GetValue("result").ToString();
            if (string.IsNullOrWhiteSpace(r))
            {
                return null;
            }

            return r.FromHexString();
        }

        public async Task<IEnumerable<GetFilterChangeResponse>> GetFilterChanges(IEnumerable<byte> filterId)
        {
            if (filterId == null)
            {
                throw new ArgumentNullException(nameof(filterId));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var jParams = new JArray();
            jParams.Add(filterId.ToHexString());
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.GetFilterChanges);
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

            var r = jsonObj.GetValue("result").ToString();
            if (string.IsNullOrWhiteSpace(r))
            {
                return null;
            }

            var jArr = JArray.Parse(r);
            if (jArr == null)
            {
                return null;
            }

            var result = new List<GetFilterChangeResponse>();
            foreach (JObject jRecord in jArr)
            {
                var jTopics = JArray.Parse(jRecord.GetValue("topics").ToString());
                var topics = new List<DataWord>();
                foreach(var jTopic in jTopics)
                {
                    topics.Add(new DataWord(jTopic.ToString().FromHexString().ToArray()));
                }

                var newRecord = new GetFilterChangeResponse
                {
                    Address = jRecord.GetValue("address").ToString().FromHexString(),
                    BlockHash = jRecord.GetValue("blockHash").ToString().FromHexString(),
                    BlockNumber = jRecord.GetValue("blockNumber").ToString().FromHexString(),
                    Data = jRecord.GetValue("data").ToString().FromHexString(),
                    Topics = topics
                };
                result.Add(newRecord);
            }

            return result;
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

        private static IEnumerable<RawMemoryPool> ExtractRawMemPool(JArray arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            var result = new List<RawMemoryPool>();
            foreach(var rec in arr)
            {
                var record = new RawMemoryPool();
                if (rec.Type == JTokenType.String)
                {
                    record.TxId = rec.ToString();
                }
                else
                {
                    var oRec = rec as JObject;
                    if (oRec == null)
                    {
                        continue;
                    }

                    record.TxId = oRec.Properties().First().Name;
                    var values = oRec.Values();
                    JObject child = null;
                    foreach(var v in values)
                    {
                        child = v as JObject;
                        break;
                    }

                    record.Fee = long.Parse(child.GetValue("fee").ToString());
                    record.ModifiedFee = long.Parse(child.GetValue("modifiedfee").ToString());
                    record.Time = int.Parse(child.GetValue("time").ToString());
                    record.AncestorCount = int.Parse(child.GetValue("ancestorcount").ToString());
                    record.DescendantCount = int.Parse(child.GetValue("descendantcount").ToString());
                }

                result.Add(record);
            }

            return result;
        }

        private Uri GetUri()
        {
            return new Uri($"http://localhost:{PortsHelper.GetRPCPort(_network)}");
        }
    }
}
