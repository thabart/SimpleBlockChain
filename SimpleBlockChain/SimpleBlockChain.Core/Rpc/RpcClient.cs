using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Factories;
using SimpleBlockChain.Core.Helpers;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Rpc
{
    public class RpcClient
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
            return BlockTemplate.Deserialize(resultObj);
        }

        public async Task<bool> SubmitBlock(Block block)
        {
            if (block == null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            var httpClient = _httpClientFactory.BuildClient();
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", Constants.RpcOperations.Submitblock);
            var parameters = new JArray();
            parameters.Add(block.Serialize().ToHexString());
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
