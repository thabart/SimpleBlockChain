using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
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
        RPC_VERIFY_ERROR = -25,
        RPC_VERIFY_REJECTED = -26,
        RPC_VERIFY_ALREADY_IN_CHAIN = -27
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
            var id = string.Empty;
            var method = string.Empty;
            IEnumerable<string> parameters = new List<string>();
            JToken idToken = null;
            JToken methodToken = null;
            JToken parametersToken = null;
            if (request.TryGetValue("id", out idToken))
            {
                id = idToken.ToString();
            }

            if (!request.TryGetValue("method", out methodToken))
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_REQUEST, "Method parameter is missing");
            } else
            {
                method = methodToken.ToString();
            }

            if (request.TryGetValue("params", out parametersToken))
            {
                var arr = parametersToken as JArray;
                if (arr != null)
                {
                    parameters = arr.Select(a => a.ToString());
                }
            }
                        
            var transactions = MemoryPool.Instance().GetTransactions();
            var blockChain = BlockChainStore.Instance().GetBlockChain();
            JObject response = CreateResponse(id);
            switch (method)
            {
                case Constants.RpcOperations.Getrawmempool: // https://bitcoin.org/en/developer-reference#getrawmempool
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

                    response["result"] = new JArray(transactions.Select(t => t.GetTxId()));
                    return response;
                case Constants.RpcOperations.Getblocktemplate: // https://bitcoin.org/en/developer-reference#getblocktemplate
                    var currentBlock = blockChain.GetCurrentBlock();
                    var height = blockChain.GetCurrentBlockHeight();
                    var previousBlockHash = currentBlock.GetHashHeader().ToHexString();
                    var transactionBuilder = new TransactionBuilder();
                    var nonce = BitConverter.GetBytes(NonceHelper.GetNonceUInt64());
                    var value = transactions.Sum(t => t.GetFee());
                    var coinBaseTransaction = transactionBuilder.NewCoinbaseTransaction()
                        .SetInput((uint)height + 1, nonce)
                        .AddOutput(value, Script.CreateCorrectScript())
                        .Build();
                    var result = new JObject();
                    var jTransactions = new JArray();
                    foreach(var transaction in transactions)
                    {
                        jTransactions.Add(transaction.Serialize().ToHexString());
                    }


                    var currentTime = DateTime.UtcNow.ToUnixTimeUInt32();
                    var coinBaseTxnObj = new JObject();
                    coinBaseTxnObj.Add("data", coinBaseTransaction.Serialize().ToHexString());
                    result.Add("coinbasetxn", coinBaseTxnObj);
                    result.Add("expires", "120");
                    result.Add("longpollid", "");
                    result.Add("height", blockChain.GetCurrentBlockHeight() + 1);
                    result.Add("curtime", currentTime);
                    result.Add("previousblockhash", previousBlockHash);
                    result.Add("transactions", jTransactions);
                    result.Add("version", Block.CURRENT_VERSION);
                    result.Add("target", TargetHelper.GetTarget(Constants.DEFAULT_NBITS).ToHexString());
                    result.Add("bits", Constants.DEFAULT_NBITS);
                    response["result"] = result;
                    return response;
                case Constants.RpcOperations.Submitblock: // https://bitcoin.org/en/developer-reference#submitblock
                    if (!parameters.Any())
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The block is missing");
                    }

                    var payload = parameters.First().FromHexString();
                    var block = Block.Deserialize(payload);
                    try
                    {
                        block.Check();
                        BlockChainStore.Instance().GetBlockChain().AddBlock(block);
                        P2PConnectorEventStore.Instance().Broadcast(block);
                        response["result"] = null;
                        return response;
                    }
                    catch(ValidationException ex)
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_VERIFY_ERROR, ex.Message);
                    }
                case Constants.RpcOperations.ListUnspent:
                    int confirmationScore = 1;
                    if (parameters.Any())
                    {
                        if (int.TryParse(parameters.First().ToString(), out confirmationScore)) { }
                    }


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
