using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Stores;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Nodes
{
    public enum RpcErrorCodes
    {
        RPC_INVALID_REQUEST = -32600,
        RPC_METHOD_NOT_FOUND = -32601,
        RPC_INVALID_PARAMS = -32602,
        RPC_PARSE_ERROR = -32700,
    }

    public class RPCNodeStartup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(context =>
            {
                return ProcessAsync(context);
            });
        }

        private async Task ProcessAsync(HttpContext context)
        {
            if (context.Request.Method != "POST") { return; } // https://bitcoin.org/en/developer-reference#remote-procedure-calls-rpcs
            JObject request = null;
            using (var reader = new StreamReader(context.Request.Body))
            {
                try
                {
                    request = JObject.Parse(reader.ReadToEnd());
                }
                catch (FormatException) { }
            }

            JObject response = null;
            if (request == null)
            {
                response = CreateErrorResponse(null, (int)RpcErrorCodes.RPC_PARSE_ERROR, "Parse error");
            }
            else
            {
                response = ProcessRequest(request);
            }

            context.Response.ContentType = "application/json-rpc";
            await context.Response.WriteAsync(response.ToString());
        }

        private JObject ProcessRequest(JObject request)
        {
            var id = request.Value<string>("id");
            var method = request.Value<string>("method");
            var parameters = request.Values<string>("params");
            if (string.IsNullOrWhiteSpace(method))
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_REQUEST, "Method parameter is missing");
            }
            
            var transactions = MemoryPool.Instance().GetTransactions();
            var blockChain = BlockChainStore.Instance().GetBlockChain();
            switch (method)
            {
                case "getrawmempool": // Get the memory POOL.
                    var verboseOutput = false;
                    if (parameters.Any())
                    {
                        var parameter = parameters.First();
                        int format = 0;
                        if (int.TryParse(parameter, out format))
                        {
                            verboseOutput = format == 0;
                        }
                    }

                    /*
                    if (verboseOutput)
                    {
                        // ADD VERBOSE OUTPUT : https://bitcoin.org/en/developer-reference#getrawmempool
                    }
                    */

                    JObject response = CreateResponse(id);
                    response["result"] = new JArray(transactions.Select(t => t.GetTxId()));
                    return response;
                case "getblocktemplate": // https://bitcoin.org/en/developer-reference#getblocktemplate
                    var currentBlock = blockChain.GetCurrentBlock();
                    var previousBlockHash = currentBlock.GetHashHeader().ToHexString();
                    var transactionBuilder = new TransactionBuilder();
                    var result = new JObject();
                    var jTransactions = new JArray();
                    foreach(var transaction in transactions)
                    {
                        jTransactions.Add(transaction.Serialize().ToHexString());
                    }

                    result.Add("expires", "");
                    result.Add("longpollid", "");
                    result.Add("height", blockChain.GetCurrentBlockHeight() + 1);
                    result.Add("curtime", "");
                    result.Add("previousblockhash", previousBlockHash);
                    result.Add("transactions", jTransactions);
                    result.Add("version", Block.CURRENT_VERSION);
                    result.Add("target", TargetHelper.GetTarget(Constants.DEFAULT_NBITS).ToString("X"));
                    result.Add("bits", Constants.DEFAULT_NBITS);
                    break;
            }

            return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_METHOD_NOT_FOUND, $"{method} Method not found");
        }

        private static JObject CreateErrorResponse(string id, int code, string message, JObject data = null)
        {
            var response = CreateResponse(id);
            response["error"] = new JObject();
            response["error"]["code"] = code;
            response["error"]["message"] = message;
            if (data != null)
            {
                response["error"]["data"] = data;
            }

            return response;
        }

        private static JObject CreateResponse(string id)
        {
            var response = new JObject();
            response["jsonrpc"] = "2.0";
            response["id"] = id;
            return response;
        }
    }
}
