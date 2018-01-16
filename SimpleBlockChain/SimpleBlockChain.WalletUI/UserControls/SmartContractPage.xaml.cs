using HashLib;
using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Rpc.Parameters;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using SimpleBlockChain.WalletUI.Helpers;
using SimpleBlockChain.WalletUI.Stores;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static SimpleBlockChain.WalletUI.ViewModels.ParameterDefinitionViewModel;

namespace SimpleBlockChain.WalletUI.UserControls
{
    public partial class SmartContractPage : UserControl
    {
        private readonly IWalletHelper _walletHelper;
        private SmartContractViewModel _viewModel;

        public SmartContractPage(IWalletHelper walletHelper)
        {
            _walletHelper = walletHelper;
            InitializeComponent();
            Loaded += Load;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
            {
                _viewModel = new SmartContractViewModel();
                DataContext = _viewModel;
                _viewModel.CallContractEvt += CallContract;
                _viewModel.CompileContractEvt += CompileContract;
                _viewModel.PublishContractEvt += PublishContract;
                _viewModel.GetSmartContractEvt += GetSmartContract;
                _viewModel.PublishTransactionCallEvt += PublishTransactionCall;
                _viewModel.GetCallValueEvt += GetCallValue;
            }
        }

        private void GetCallValue(object sender, EventArgs e)
        {
            var selectedFnDef = _viewModel.SelectedFunctionDefinition;
            if (selectedFnDef == null)
            {
                MainWindowStore.Instance().DisplayError("A function must be selected");
                return;
            }

            var parameters = string.Empty;
            if (selectedFnDef.Parameters != null)
            {
                parameters = string.Join(",", selectedFnDef.Parameters.Select(p => p.Type));
            }

            var operationName = string.Format("{0}({1})", selectedFnDef.FunctionName, parameters);
            var hash = HashFactory.Crypto.SHA3.CreateKeccak256();
            var callDataValue = new List<byte>();
            var operationPayload = hash.ComputeBytes(Encoding.ASCII.GetBytes(operationName)).GetBytes().Take(4);
            callDataValue.AddRange(operationPayload);
            if (selectedFnDef.Parameters != null)
            {
                for(var i = 1; i <= selectedFnDef.Parameters.Count; i++)
                {
                    var p = selectedFnDef.Parameters[i - 1];
                    callDataValue.AddRange(new DataWord(32 * (i * selectedFnDef.Parameters.Count)).GetData());
                }

                foreach (var p in selectedFnDef.Parameters)
                {
                    callDataValue.AddRange(new DataWord(Encoding.ASCII.GetBytes(p.Value).Count()).GetData());
                    callDataValue.AddRange(new DataWord(Encoding.ASCII.GetBytes(p.Value)).GetReverseData());
                }
            }

            var hex = callDataValue.ToHexString();
            Application.Current.Dispatcher.Invoke(() => _viewModel.GeneratedCallValue = hex);
        }

        private void PublishTransactionCall(object sender, EventArgs e)
        {

            if (_viewModel == null) { return; }
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                MainWindowStore.Instance().DisplayError("You're not authenticated");
                return;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.SmartContractAddress))
            {
                MainWindowStore.Instance().DisplayError("The address should be filled in");
                return;
            }

            IEnumerable<byte> to = null;
            IEnumerable<byte> data = null;
            try
            {
                to = _viewModel.SmartContractAddress.FromHexString();
            }
            catch
            {
                MainWindowStore.Instance().DisplayError("The address should be encoded in hex");
                return;
            }

            if (!string.IsNullOrWhiteSpace(_viewModel.SmartContractCallValue))
            {
                try
                {
                    data = _viewModel.SmartContractCallValue.FromHexString();
                }
                catch
                {
                    MainWindowStore.Instance().DisplayError("The callvalue should be encoded in hex");
                    return;
                }
            }
            
            var rpcClient = new RpcClient(authenticatedWallet.Network);
            var smartContractTransaction = new SmartContractTransaction
            {
                To = to,
                Data = data
            };
            rpcClient.SendRawTransaction(smartContractTransaction).ContinueWith((t) =>
            {
                try
                {
                    var txId = t.Result;
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayMessage(string.Format("A new transaction has been published : {0}", txId)));
                }
                catch (AggregateException)
                {
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying to publish the transaction"));
                }
            });
        }

        private void CallContract(object sender, EventArgs e)
        {
            if (_viewModel == null) { return; }
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                MainWindowStore.Instance().DisplayError("You're not authenticated");
                return;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.SmartContractAddress))
            {
                MainWindowStore.Instance().DisplayError("The address should be filled in");
                return;
            }

            IEnumerable<byte> to = null;
            IEnumerable<byte> data = null;
            try
            {
                to = _viewModel.SmartContractAddress.FromHexString();
            }
            catch
            {
                MainWindowStore.Instance().DisplayError("The address should be encoded in hex");
                return;
            }

            if (!string.IsNullOrWhiteSpace(_viewModel.SmartContractCallValue))
            {
                try
                {
                    data = _viewModel.SmartContractCallValue.FromHexString();
                }
                catch
                {
                    MainWindowStore.Instance().DisplayError("The callvalue should be encoded in hex");
                    return;
                }
            }

            var smartContractTransactionParameter = new SmartContractTransactionParameter(to);
            if (data != null)
            {
                smartContractTransactionParameter.Data = data;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.CallSmartContract(smartContractTransactionParameter).ContinueWith((t) =>
            {
                try
                {
                    var smartContractResult = t.Result;
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayMessage(string.Format("Result of the operation : {0}", smartContractResult)));
                }
                catch(AggregateException)
                {
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying to call the contract"));
                }
            });
        }

        private void CompileContract(object sender, EventArgs e)
        {
            if (_viewModel == null) { return; }
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                MainWindowStore.Instance().DisplayError("You're not authenticated");
                return;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.SmartContract))
            {
                MainWindowStore.Instance().DisplayError("The solidity contract must be filled in");
                return;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.CompileSolidity(_viewModel.SmartContract).ContinueWith((t) =>
            {
                try
                {
                    var compilationResult = t.Result;
                    if (compilationResult.Infos == null)
                    {
                        return;
                    }

                    Application.Current.Dispatcher.Invoke(() => _viewModel.FunctionDefinitions.Clear());
                    foreach (var info in compilationResult.Infos)
                    {
                        var abiDefinition = info.AbiDefinition;
                        foreach(JObject record in abiDefinition)
                        {
                            JToken jTokenName = null;
                            if (!record.TryGetValue("name", out jTokenName))
                            {
                                continue;
                            }

                            JToken jTokenInputs = null;
                            var fnType = record.GetValue("type").ToString();
                            if (fnType != "function")
                            {
                                continue;
                            }

                            var parameters = new ObservableCollection<ParameterDefinitionViewModel>();
                            if (record.TryGetValue("inputs", out jTokenInputs))
                            {
                                var jArrInput = jTokenInputs as JArray;
                                if (jArrInput != null)
                                {
                                    foreach(JObject inputDef in jArrInput)
                                    {
                                        parameters.Add(new ParameterDefinitionViewModel
                                        {
                                            Name = inputDef.GetValue("name").ToString(),
                                            Type = inputDef.GetValue("type").ToString()
                                        });
                                    }
                                }
                            }

                            var newFuncDef = new FunctionDefinitionViewModel
                            {
                                FunctionName = jTokenName.ToString(),
                                Parameters = parameters  
                            };
                            Application.Current.Dispatcher.Invoke(() => _viewModel.FunctionDefinitions.Add(newFuncDef));
                        }
                        
                        Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayMessage("The contract has been compiled"));
                    }
                }
                catch (AggregateException)
                {
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying to build the solidity contract"));
                }
            });
        }

        private void PublishContract(object sender, EventArgs e)
        {
            if (_viewModel == null) { return; }
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                MainWindowStore.Instance().DisplayError("You're not authenticated");
                return;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.SmartContract))
            {
                MainWindowStore.Instance().DisplayError("The solidity contract must be filled in");
                return;
            }
            
            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.CompileSolidity(_viewModel.SmartContract).ContinueWith((t) =>
            {
                try
                {
                    var compilationResult = t.Result;
                    if (compilationResult.Infos == null || !compilationResult.Infos.Any())
                    {
                        return;
                    }

                    var newKey = _walletHelper.CreateNewAddress();
                    var fromAddr = newKey.GetPublicKeyHashed();
                    var smartContractTransaction = new SmartContractTransaction
                    {
                        From = fromAddr,
                        Data = compilationResult.Infos.First().Code.FromHexString(),
                        Nonce = NonceHelper.GetNonceInt32()
                    };
                    rpcClient.SendRawTransaction(smartContractTransaction).ContinueWith((c) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _viewModel.TransactionId = c.Result;
                        });
                    });
                }
                catch (AggregateException)
                {
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying to build the solidity contract"));
                }
            });
        }

        private void GetSmartContract(object sender, EventArgs e)
        {
            if (_viewModel == null) { return; }
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                MainWindowStore.Instance().DisplayError("You're not authenticated");
                return;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.TransactionAddress))
            {
                MainWindowStore.Instance().DisplayError("The transaction address must be filled in");
                return;
            }

            IEnumerable<byte> txId = null;
            try
            {
                txId = _viewModel.TransactionAddress.FromHexString();
            }
            catch(Exception)
            {
                MainWindowStore.Instance().DisplayError("The transaction address is not a valid hex");
                return;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.GetTransactionReceipt(txId).ContinueWith((r) =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _viewModel.GetSmartContractAddressResult = r.Result.ContractAddress;
                    });
                }
                catch(AggregateException)
                {
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying to get the smart contract address"));
                }                
            });
        }
    }
}
