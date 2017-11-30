using Newtonsoft.Json.Linq;
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
    }
}
