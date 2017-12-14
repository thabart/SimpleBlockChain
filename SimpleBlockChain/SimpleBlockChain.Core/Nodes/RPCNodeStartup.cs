﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlockChain.Core.Nodes
{
    internal enum RpcErrorCodes
    {
        RPC_INVALID_REQUEST = -32600,
        RPC_METHOD_NOT_FOUND = -32601,
        RPC_INVALID_PARAMS = -32602,
        RPC_PARSE_ERROR = -32700,
        RPC_VERIFY_ERROR = -25,
        RPC_VERIFY_REJECTED = -26,
        RPC_VERIFY_ALREADY_IN_CHAIN = -27,
        RPC_WALLET_NOT_FOUND = -18
    }

    internal class RPCNodeStartup
    {
        private readonly IWalletRepository _walletRepository;
        private readonly Networks _network;

        public RPCNodeStartup(IWalletRepository walletRepository, Networks network)
        {
            _walletRepository = walletRepository;
            _network = network;
        }

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
            var wallet = WalletStore.Instance().GetAuthenticatedWallet();
            JObject response = CreateResponse(id);
            switch (method)
            {
                case Constants.RpcOperations.Getrawmempool: // https://bitcoin.org/en/developer-reference#getrawmempool
                    var verboseOutput = false;
                    if (parameters.Any())
                    {
                        var parameter = parameters.First();
                        int f = 0;
                        if (int.TryParse(parameter, out f))
                        {
                            verboseOutput = f == 0;
                        }
                    }
                    response["result"] = new JArray(transactions.Select(t => t.GetTxId()));
                    return response;
                case Constants.RpcOperations.Getblocktemplate: // https://bitcoin.org/en/developer-reference#getblocktemplate
                    if (transactions == null || !transactions.Any())
                    {
                        response["result"] = null;
                        return response;
                    }

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
                    foreach (var transaction in transactions)
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
                    result.Add("version", BlockHeader.CURRENT_VERSION);
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
                        if (block.Transactions != null)
                        {
                            MemoryPool.Instance().Remove(block.Transactions.Select(tx => tx.GetTxId()));
                        }

                        response["result"] = null;
                        return response;
                    }
                    catch (ValidationException ex)
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_VERIFY_ERROR, ex.Message);
                    }
                case Constants.RpcOperations.ListUnspent: // https://bitcoin.org/en/developer-reference#listunspent
                    int confirmationScore = 1;
                    var maxConfirmations = 9999999;
                    IEnumerable<string> addrs = new List<string>();
                    if (parameters.Any())
                    {
                        if (int.TryParse(parameters.First().ToString(), out confirmationScore)) { }
                        if (parameters.Count() >= 2 && int.TryParse(parameters.ElementAt(1), out maxConfirmations)) { }
                        if (parameters.Count() >= 3)
                        {
                            var jArr = JArray.Parse(parameters.ElementAt(2));
                            if (jArr != null)
                            {
                                addrs = jArr.Select(j => j.ToString());
                            }
                        }
                    }


                    if (wallet == null)
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_WALLET_NOT_FOUND, "No authenticated wallet");
                    }

                    var res = new JArray();
                    var unspentTxIds = blockChain.GetUnspentTransactions();
                    if (addrs == null || !addrs.Any())
                    {
                        addrs = wallet.Addresses.Select(a => a.Hash);
                    }

                    if (unspentTxIds != null)
                    {
                        foreach (var unspentTxId in unspentTxIds)
                        {
                            var transaction = blockChain.GetUnspentTransaction(unspentTxId);
                            if (addrs.Any())
                            {
                                foreach (var hash in addrs)
                                {
                                    var txOut = transaction.GetTransactionOut(hash);
                                    if (txOut != null)
                                    {
                                        var record = new JObject();
                                        record.Add("txid", transaction.GetTxId().ToHexString());
                                        record.Add("vout", transaction.TransactionOut.IndexOf(txOut));
                                        record.Add("address", hash);
                                        record.Add("scriptPubKey", txOut.Script.Serialize().ToHexString());
                                        record.Add("amount", txOut.Value);
                                        record.Add("confirmations", 0);
                                        record.Add("spendable", wallet.Addresses.Select(a => a.Hash).Contains(hash));
                                        record.Add("solvable", true);
                                        res.Add(record);
                                    }
                                }
                            }
                        }
                    }

                    response["result"] = res;
                    return response;
                case Constants.RpcOperations.SendRawTransaction: // https://bitcoin.org/en/developer-reference#sendrawtransaction
                    if (parameters == null || !parameters.Any())
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The transaction is missing");
                    }

                    var txPayload = parameters.First().FromHexString();
                    var allowHighFees = false;
                    TransactionTypes transactionType = TransactionTypes.NoneCoinbase;
                    if (parameters.Count() >= 2)
                    {
                        if (bool.TryParse(parameters.ElementAt(1), out allowHighFees)) { }
                    }

                    if (parameters.Count() >= 3)
                    {
                        if (parameters.ElementAt(2).ToLower() == "coinbase")
                        {
                            transactionType = TransactionTypes.Coinbase;
                        }
                    }

                    var kvp = BaseTransaction.Deserialize(txPayload, transactionType);
                    try
                    {
                        var tx = kvp.Key;
                        tx.Check();
                        MemoryPool.Instance().AddTransaction(tx);
                        P2PConnectorEventStore.Instance().Broadcast(tx);
                        response["result"] = tx.GetTxId().ToHexString();
                        return response;
                    }
                    catch (ValidationException ex)
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_VERIFY_ERROR, ex.Message);
                    }
                /*
                case Constants.RpcOperations.GetMempoolEntry: // https://bitcoin.org/en/developer-reference#getmempoolentry
                    if (parameters == null || !parameters.Any())
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The address is missing");
                    }

                    var address = parameters.First();
                    var memTransactions = MemoryPool.Instance().GetTransactions();

                    break;
                */
                case Constants.RpcOperations.GetUnconfirmedBalance: // https://bitcoin.org/en/developer-reference#getunconfirmedbalance
                    if (wallet == null)
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_WALLET_NOT_FOUND, "No authenticated wallet");
                    }

                    long unconfirmedBalance = 0;
                    if (wallet.Addresses != null)
                    {
                        foreach(var adr in wallet.Addresses)
                        {
                            foreach(var memTx in transactions)
                            {
                                var balance = memTx.CalculateBalance(adr.Hash);
                                unconfirmedBalance += balance;
                            }
                        }
                    }

                    response["result"] = unconfirmedBalance;
                    return response;
                case Constants.RpcOperations.GetBlockCount: // https://bitcoin.org/en/developer-reference#getblockcount
                    response["result"] = blockChain.GetCurrentBlockHeight() + 1;
                    return response;
                case Constants.RpcOperations.GetBlockHash: // https://bitcoin.org/en/developer-reference#getblockhash
                    if (parameters == null || !parameters.Any())
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "Block height must be specified");
                    }

                    var str = parameters.First();
                    var blockHeight = 0;
                    if (!int.TryParse(str, out blockHeight))
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "Block height is not a number");
                    }
                    
                    var requestedBlock = blockChain.GetBlock(blockHeight);
                    if (requestedBlock == null)
                    {
                        response["result"] = null;
                    }
                    else
                    {
                        var h2 = requestedBlock.GetHashHeader();
                        response["result"] = requestedBlock.GetHashHeader().ToHexString();
                    }

                    return response;
                case Constants.RpcOperations.GetBlock: // https://bitcoin.org/en/developer-reference#getblock
                    if (parameters == null || !parameters.Any())
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The block header hash is not specified");
                    }

                    var blockHeaderHash = parameters.First();
                    var format = 0;
                    if (parameters.Count() >= 2)
                    {
                        if (int.TryParse(parameters.ElementAt(1), out format)) { }
                        if (format != 0 || format != 1) { format = 0; }
                    }

                    IEnumerable<byte> blockHeader = null;
                    try
                    {
                        blockHeader = blockHeaderHash.FromHexString();
                    }
                    catch (Exception)
                    {
                        return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "Cannot deserialize block header hash into byte array");
                    }

                    var recordBlock = blockChain.GetBlock(blockHeader);
                    if (recordBlock == null)
                    {
                        response["result"] = null;
                        return response;
                    }
                    
                    var h = recordBlock.GetHashHeader();
                    response["result"] = recordBlock.Serialize().ToHexString();
                    return response;
            }

            return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_METHOD_NOT_FOUND, $"{method} Method not found");
        }

        private static JObject SerializeTransaction()
        {
            return null;
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
