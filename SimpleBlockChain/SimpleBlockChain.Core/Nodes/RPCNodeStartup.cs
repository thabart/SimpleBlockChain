using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Blocks;
using SimpleBlockChain.Core.Builders;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.Core.Validators;
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
        private readonly IBlockChainStore _blockChainStore;
        private readonly ISmartContractStore _smartContractStore;
        private readonly ITransactionHelper _transactionHelper;
        private readonly ITransactionValidator _transactionValidator;
        private readonly IBlockValidator _blockValidator;

        public RPCNodeStartup(IWalletRepository walletRepository, Networks network, IBlockChainStore blockChainStore, 
            ISmartContractStore smartContractStore, ITransactionHelper transactionHelper, ITransactionValidator transactionValidator, IBlockValidator blockValidator)
        {
            _walletRepository = walletRepository;
            _network = network;
            _blockChainStore = blockChainStore;
            _smartContractStore = smartContractStore;
            _transactionHelper = transactionHelper;
            _transactionValidator = transactionValidator;
            _blockValidator = blockValidator;
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

            JObject response = CreateResponse(id);
            switch (method)
            {
                case Constants.RpcOperations.Getrawmempool: // https://bitcoin.org/en/developer-reference#getrawmempool
                    return GetRawMemPool(parameters, response);
                case Constants.RpcOperations.Getblocktemplate: // https://bitcoin.org/en/developer-reference#getblocktemplate
                    return GetBlockTemplate(response);
                case Constants.RpcOperations.Submitblock: // https://bitcoin.org/en/developer-reference#submitblock
                    return SubmitBlock(parameters, id, response);
                case Constants.RpcOperations.ListUnspent: // https://bitcoin.org/en/developer-reference#listunspent
                    return ListUnspent(parameters, response, id);
                case Constants.RpcOperations.SendRawTransaction: // https://bitcoin.org/en/developer-reference#sendrawtransaction
                    return SendRawTransaction(parameters, response, id);
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
                    return GetUnconfirmedBalance(id, response);
                case Constants.RpcOperations.GetBlockCount: // https://bitcoin.org/en/developer-reference#getblockcount
                    return GetBlockCount(response);
                case Constants.RpcOperations.GetBlockHash: // https://bitcoin.org/en/developer-reference#getblockhash
                    return GetBlockHash(parameters, response, id);
                case Constants.RpcOperations.GetBlock: // https://bitcoin.org/en/developer-reference#getblock
                    return GetBlock(parameters, id, response);
                case Constants.RpcOperations.GetRawTransaction: // https://bitcoin.org/en/developer-reference#getrawtransaction
                    return GetRawTransaction(parameters, response, id);
                case Constants.RpcOperations.ScSendTransaction: https://github.com/ethereum/wiki/wiki/JSON-RPC#eth_sendtransaction
                    return SendSmartContractTransaction(parameters, id, response);
                case Constants.RpcOperations.ScCall: // https://github.com/ethereum/wiki/wiki/JSON-RPC#eth_call
                    return CallSmartContract(parameters, id, response);
                case Constants.RpcOperations.CompileSolidity: // https://github.com/ethereum/wiki/wiki/JSON-RPC#eth_compilesolidity
                    return CompileSolidity(parameters, id, response);
                case Constants.RpcOperations.GetCompilers:
                    return GetCompilers(response);
                case Constants.RpcOperations.GetTransactionReceipt:
                    return GetTransactionReceipt(parameters, id, response);
            }

            return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_METHOD_NOT_FOUND, $"{method} Method not found");
        }

        #region BIT COIN CORE OPERATIONS

        private JObject GetRawMemPool(IEnumerable<string> parameters, JObject response)
        {
            var memPool = MemoryPool.Instance();
            var transactions = memPool.GetTransactions();
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

            if (!verboseOutput)
            {
                response["result"] = new JArray(transactions.Select(t => t.Transaction.GetTxId()));
                return response;
            }

            JArray jTxs = new JArray();
            if (transactions != null)
            {
                foreach (var transaction in transactions.Select(t => t))
                {
                    var jTxContentObj = new JObject();
                    var depends = new List<string>();
                    transaction.GetDepends(depends);
                    var bcTx = transaction.Transaction as BcBaseTransaction;
                    var arrDepends = JArray.FromObject(depends);
                    jTxContentObj.Add("size", transaction.Transaction.Serialize().Count());
                    jTxContentObj.Add("fee", bcTx == null ? 0 : _transactionHelper.GetFee(bcTx, _network));
                    jTxContentObj.Add("modifiedfee", bcTx == null ? 0 : _transactionHelper.GetFee(bcTx, _network));
                    jTxContentObj.Add("time", transaction.InsertTime.ToUnixTime());
                    jTxContentObj.Add("height", transaction.BlockHeight);
                    jTxContentObj.Add("startingpriority", null);
                    jTxContentObj.Add("currentpriority", null);
                    jTxContentObj.Add("descendantcount", memPool.CountDescendants(transaction)); // Nombre of descendants.
                    jTxContentObj.Add("descendantsize", null);
                    jTxContentObj.Add("descendantfees", null);
                    jTxContentObj.Add("ancestorcount", memPool.CountAncestors(transaction));
                    jTxContentObj.Add("ancestorsize", null);
                    jTxContentObj.Add("ancestorfees", null);
                    jTxContentObj.Add("depends", arrDepends);
                    var rec = new JObject();
                    rec.Add(transaction.Transaction.GetTxId().ToHexString(), jTxContentObj);
                    jTxs.Add(rec);
                }
            }

            response["result"] = jTxs;
            return response;
        }

        private JObject GetBlockTemplate(JObject response)
        {
            var transactions = MemoryPool.Instance().GetTransactions();
            var blockChain = _blockChainStore.GetBlockChain();
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
            var value = transactions.Sum(t => _transactionHelper.GetFee(t.Transaction, _network));
            var coinBaseTransaction = transactionBuilder.NewCoinbaseTransaction()
                .SetInput((uint)height + 1, nonce)
                .AddOutput(value, Script.CreateCorrectScript())
                .Build();
            var result = new JObject();
            var jTransactions = new JArray();
            foreach (var transaction in transactions)
            {
                jTransactions.Add(transaction.Transaction.Serialize().ToHexString());
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
        }

        private JObject SubmitBlock(IEnumerable<string> parameters, string id, JObject response)
        {
            var blockChain = _blockChainStore.GetBlockChain();
            var smartContract = _smartContractStore.GetSmartContracts();
            if (!parameters.Any())
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The block is missing");
            }

            var payload = parameters.First().FromHexString();
            var block = Block.Deserialize(payload);
            try
            {
                _blockValidator.Check(block);
                blockChain.AddBlock(block);
                smartContract.AddBlock(block);
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
        }

        private JObject ListUnspent(IEnumerable<string> parameters, JObject response, string id)
        {
            var transactions = MemoryPool.Instance().GetTransactions();
            var blockChain = _blockChainStore.GetBlockChain();
            var wallet = WalletStore.Instance().GetAuthenticatedWallet();
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
            if (addrs == null || !addrs.Any())
            {
                addrs = wallet.Addresses.Select(a => a.Hash);
            }

            var walletBlockChainAddrs = addrs.Select(a => new { bca = BlockChainAddress.Deserialize(a), hash = a });
            if (maxConfirmations >= 0) // CONFIRMATION 0.
            {
                if (transactions != null && transactions.Any())
                {
                    foreach (var unconfirmedTransaction in transactions)
                    {
                        if (unconfirmedTransaction.Transaction != null)
                        {
                            var lBcTx = unconfirmedTransaction.Transaction as BcBaseTransaction;
                            if (lBcTx == null)
                            {
                                continue;
                            }

                            foreach (var unconfirmedUTXO in lBcTx.TransactionOut.Where(t => t is TransactionOut).Select(t => t as TransactionOut))
                            {
                                var bcAdr = walletBlockChainAddrs.FirstOrDefault(wph => unconfirmedUTXO.Script.ContainsPublicKeyHash(wph.bca.PublicKeyHash));
                                if (bcAdr == null)
                                {
                                    continue;
                                }

                                var record = new JObject();
                                record.Add("txid", unconfirmedTransaction.Transaction.GetTxId().ToHexString());
                                record.Add("vout", lBcTx.TransactionOut.IndexOf(unconfirmedUTXO));
                                record.Add("amount", unconfirmedUTXO.Value);
                                record.Add("address", bcAdr.hash);
                                record.Add("scriptPubKey", unconfirmedUTXO.Script.Serialize().ToHexString());
                                record.Add("confirmations", 0);
                                record.Add("spendable", true);
                                record.Add("solvable", true);
                                res.Add(record);
                            }
                        }
                    }
                }
            }

            if (maxConfirmations >= 1)  // CONFIRMATION 1.
            {
                var utxos = blockChain.GetUnspentTransactions();
                foreach (var utxo in utxos)
                {
                    var bcAdr = walletBlockChainAddrs.FirstOrDefault(wph => utxo.Script.ContainsPublicKeyHash(wph.bca.PublicKeyHash));
                    if (bcAdr == null)
                    {
                        continue;
                    }

                    var record = new JObject();
                    record.Add("txid", utxo.TxId.ToHexString());
                    record.Add("vout", utxo.Index);
                    record.Add("address", bcAdr.hash);
                    record.Add("scriptPubKey", utxo.Script.Serialize().ToHexString());
                    record.Add("amount", utxo.Value);
                    record.Add("confirmations", 1);
                    record.Add("spendable", true);
                    record.Add("solvable", true);
                    res.Add(record);
                }
            }

            response["result"] = res;
            return response;
        }

        private JObject SendRawTransaction(IEnumerable<string> parameters, JObject response, string id)
        {
            var blockChain = _blockChainStore.GetBlockChain();
            if (parameters == null || !parameters.Any())
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The transaction is missing");
            }

            var txPayload = parameters.First().FromHexString();
            var allowHighFees = false;
            if (parameters.Count() >= 2)
            {
                if (bool.TryParse(parameters.ElementAt(1), out allowHighFees)) { }
            }

            var kvp = BaseTransaction.Deserialize(txPayload);
            try
            {
                var tx = kvp.Key;
                _transactionValidator.Check(tx);
                MemoryPool.Instance().AddTransaction(tx, blockChain.GetCurrentBlockHeight());
                P2PConnectorEventStore.Instance().Broadcast(tx);
                response["result"] = tx.GetTxId().ToHexString();
                return response;
            }
            catch (ValidationException ex)
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_VERIFY_ERROR, ex.Message);
            }
        }

        private JObject GetUnconfirmedBalance(string id, JObject response)
        {
            var transactions = MemoryPool.Instance().GetTransactions();
            var wallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (wallet == null)
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_WALLET_NOT_FOUND, "No authenticated wallet");
            }

            long unconfirmedBalance = 0;
            if (wallet.Addresses != null)
            {
                var bcAddrs = wallet.Addresses.Select(addr => BlockChainAddress.Deserialize(addr.Hash));
                foreach (var memTx in transactions)
                {
                    var mBcTx = memTx.Transaction as BcBaseTransaction;
                    if (mBcTx == null)
                    {
                        continue;
                    }

                    var balance = _transactionHelper.CalculateBalance(mBcTx, bcAddrs, _network);
                    unconfirmedBalance += balance;
                }
            }

            response["result"] = unconfirmedBalance;
            return response;
        }

        private JObject GetBlockCount(JObject response)
        {
            var blockChain = _blockChainStore.GetBlockChain();
            response["result"] = blockChain.GetCurrentBlockHeight() + 1;
            return response;
        }

        private JObject GetBlockHash(IEnumerable<string> parameters, JObject response, string id)
        {
            var blockChain = _blockChainStore.GetBlockChain();
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
                response["result"] = requestedBlock.GetHashHeader().ToHexString();
            }

            return response;
        }

        private JObject GetBlock(IEnumerable<string> parameters, string id, JObject response)
        {
            var blockChain = _blockChainStore.GetBlockChain();
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

        private JObject GetRawTransaction(IEnumerable<string> parameters, JObject response, string id)
        {
            var blockChain = _blockChainStore.GetBlockChain();
            if (parameters == null || !parameters.Any())
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The txid is not specified");
            }

            var txIdStr = parameters.First();
            bool txFormat = false;
            if (parameters.Count() > 2)
            {
                var txFormatStr = parameters.ElementAt(1);
                if (bool.TryParse(txFormatStr, out txFormat)) { }
            }

            IEnumerable<byte> txIdPayload;
            try
            {
                txIdPayload = txIdStr.FromHexString();
            }
            catch (Exception)
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The txid cannot be decoded");
            }

            var txG = blockChain.GetTransaction(txIdPayload);
            if (txG == null)
            {
                response["result"] = null;
                return response;
            }

            response["result"] = txG.Serialize().ToHexString();
            return response;
        }

        #endregion

        #region SMART CONTRACT OPERATIONS

        private JObject SendSmartContractTransaction(IEnumerable<string> parameters, string id, JObject response)
        {
            var blockChain = _blockChainStore.GetBlockChain();
            if (parameters == null || !parameters.Any())
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The from & data are not specified");
            }

            if (parameters.Count() < 2)
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The data is not specified");
            }

            IEnumerable<byte> fromPayload = null;
            IEnumerable<byte> dataPayload = null;
            IEnumerable<byte> toPayload = null;
            double gas = 0,
                gasPrice = 0,
                scValue = 0;
            int scNonce = 0;
            try
            {
                fromPayload = parameters.First().FromHexString();
            }
            catch (Exception)
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The from cannot be decoded");
            }

            try
            {
                dataPayload = parameters.ElementAt(1).FromHexString();
            }
            catch (Exception)
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The data cannot be decoded");
            }

            if (parameters.Count() >= 3)
            {
                try
                {
                    toPayload = parameters.ElementAt(2).FromHexString();
                }
                catch (Exception)
                {
                    return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The to cannot be decoded");
                }
            }

            if (parameters.Count() >= 4)
            {
                try
                {
                    gas = double.Parse(parameters.ElementAt(3));
                }
                catch (Exception) { }
            }

            if (parameters.Count() >= 5)
            {
                try
                {
                    gasPrice = double.Parse(parameters.ElementAt(4));
                }
                catch (Exception) { }
            }

            if (parameters.Count() >= 6)
            {
                try
                {
                    scValue = double.Parse(parameters.ElementAt(5));
                }
                catch (Exception) { }
            }

            if (parameters.Count() >= 7)
            {
                try
                {
                    scNonce = int.Parse(parameters.ElementAt(6));
                }
                catch (Exception) { }
            }

            var smartContractTx = new SmartContractTransaction
            {
                Data = dataPayload,
                From = fromPayload,
                To = toPayload,
                Gas = gas,
                GasPrice = gasPrice,
                Nonce = scNonce,
                Value = scValue
            };

            try
            {
                _transactionValidator.Check(smartContractTx);
                MemoryPool.Instance().AddTransaction(smartContractTx, blockChain.GetCurrentBlockHeight());
                P2PConnectorEventStore.Instance().Broadcast(smartContractTx);
                response["result"] = smartContractTx.GetTxId().ToHexString();
                return response;
            }
            catch (ValidationException ex)
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_VERIFY_ERROR, ex.Message);
            }
        }

        private JObject CallSmartContract(IEnumerable<string> parameters, string id, JObject response)
        {
            if (parameters == null || !parameters.Any())
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The to is not specified");
            }

            IEnumerable<byte> callToPayload = null;
            IEnumerable<byte> callFromPayload = null;
            IEnumerable<byte> callDataPayload = null;
            double callGas = 0,
                callGasPrice = 0,
                callScValue = 0;
            try
            {
                callToPayload = parameters.First().FromHexString(); // TO
            }
            catch (Exception)
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The to cannot be decoded");
            }

            if (parameters.Count() > 2)
            {
                try
                {
                    callFromPayload = parameters.ElementAt(1).FromHexString(); // FROM
                }
                catch (Exception)
                {
                    return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The from cannot be decoded");
                }
            }

            if (parameters.Count() >= 3)
            {
                try
                {
                    callDataPayload = parameters.ElementAt(2).FromHexString(); // DATA
                }
                catch (Exception)
                {
                    return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The data cannot be decoded");
                }
            }

            if (parameters.Count() >= 4)
            {
                try
                {
                    callGas = double.Parse(parameters.ElementAt(3)); // GAS
                }
                catch (Exception) { }
            }

            if (parameters.Count() >= 5)
            {
                try
                {
                    callGasPrice = double.Parse(parameters.ElementAt(4)); // GASPRICE
                }
                catch (Exception) { }
            }

            if (parameters.Count() >= 6)
            {
                try
                {
                    callScValue = double.Parse(parameters.ElementAt(5)); // VALUE
                }
                catch (Exception) { }
            }

            var callSmartContractTx = new SmartContractTransaction
            {
                Data = callDataPayload,
                From = callFromPayload,
                To = callToPayload,
                Gas = callGas,
                GasPrice = callGasPrice,
                Value = callScValue
            };
            try
            {
                _transactionValidator.Check(callSmartContractTx);
                var smartContract = _smartContractStore.GetSmartContracts().GetSmartContract(callToPayload);
                var fromAddr = new DataWord();
                if (callFromPayload != null)
                {
                    fromAddr = new DataWord(callFromPayload.ToArray());
                }

                var defaultCallValue = new DataWord(new byte[] { 0x00 });
                var program = new SolidityProgram(smartContract.Code.ToList(), new SolidityProgramInvoke(callDataPayload, 
                    smartContract.Address, fromAddr, defaultCallValue, _smartContractStore.GetSmartContracts()));
                var vm = new SolidityVm();
                while (!program.IsStopped())
                {
                    vm.Step(program);
                }

                var result = program.GetResult().GetHReturn().ToHexString();
                response["result"] = result;
                return response;
            }
            catch (ValidationException ex)
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_VERIFY_ERROR, ex.Message);
            }
            catch (Exception)
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_VERIFY_ERROR, ErrorCodes.ErrorExecutionContract);
            }
        }

        public JObject CompileSolidity(IEnumerable<string> parameters, string id, JObject response)
        {
            if (parameters == null || !parameters.Any())
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The source is not specified");
            }

            var contract = parameters.First();
            IEnumerable<SolidityCompilerResult> compilationResult;
            try
            {
                compilationResult = SolidityCompiler.Compile(contract);
            }
            catch
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_PARSE_ERROR, "the contract cannot be parsed");
            }


            var result = new JObject();
            result.Add("language", "Solidity");
            result.Add("languageVersion", "0");
            result.Add("compilerVersion", "1.7.3");
            result.Add("source", contract);
            var info = new JArray();
            foreach(var compilationR in compilationResult)
            {
                var record = new JObject();
                record.Add("abiDefinition", compilationR.AbiCode);
                record.Add("code", compilationR.Payload);
                info.Add(record);
            }

            result.Add("info", info);
            response["result"] = result;
            return response;
        }

        public JObject GetCompilers(JObject response)
        {
            var compilers = new JArray();
            compilers.Add("solidity");
            response["result"] = compilers;
            return response;
        }

        private JObject GetTransactionReceipt(IEnumerable<string> parameters, string id, JObject response)
        {
            if (parameters == null || !parameters.Any())
            {
                return CreateErrorResponse(id, (int)RpcErrorCodes.RPC_INVALID_PARAMS, "The hash is not specified");
            }

            return response;
        }

        #endregion

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
