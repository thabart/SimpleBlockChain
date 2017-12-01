using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Helpers;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Rpc
{
    public class RpcClient
    {
        private Networks _network;

        public RpcClient(Networks network)
        {
            _network = network;
        }
        
        public async Task<string> GetRawMemPool()
        {
            var httpClient = new HttpClient();
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", "getrawmempool");
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, "application/json-rpc");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = new Uri($"http://localhost:{PortsHelper.GetRPCPort(_network)}")
            };

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<BlockTemplate> GetBlockTemplate()
        {
            var httpClient = new HttpClient();
            var jObj = new JObject();
            jObj.Add("id", Guid.NewGuid().ToString());
            jObj.Add("method", "getblocktemplate");
            var content = new StringContent(jObj.ToString(), System.Text.Encoding.UTF8, "application/json-rpc");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = content,
                RequestUri = new Uri($"http://localhost:{PortsHelper.GetRPCPort(_network)}")
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
    }
}
