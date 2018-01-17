using SimpleBlockChain.Core.Aggregates;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Repositories;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Rpc.Parameters;
using SimpleBlockChain.Core.Rpc.Responses;
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

namespace SimpleBlockChain.WalletUI.UserControls
{
    public partial class SmartContractPage : UserControl
    {
        private readonly IWalletHelper _walletHelper;
        private readonly ISolidityContractsRepository _solidityContractsRepository;
        private readonly ISolidityFilterRepository _solidityFilterRepository;
        private SmartContractViewModel _viewModel;
        private SolidityContractAggregate _publishedSolidityContract = null;

        public SmartContractPage(IWalletHelper walletHelper, ISolidityContractsRepository solidityContractsRepository, ISolidityFilterRepository solidityFilterRepository)
        {
            _walletHelper = walletHelper;
            _solidityContractsRepository = solidityContractsRepository;
            _solidityFilterRepository = solidityFilterRepository;
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
                _viewModel.PublishTransactionCallEvt += PublishTransactionCall;
                _viewModel.PersistSmartContractEvt += PersistSmartContract;
                _viewModel.RefreshContractsEvt += Refresh;
                _viewModel.ListenSmartContractEvt += ListenSmartContract;
                _viewModel.GetLastLogsEvt += GetLastLogs;
            }
        }

        private void GetLastLogs(object sender, EventArgs e)
        {
            if (_viewModel == null) { return; }
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                MainWindowStore.Instance().DisplayError("You're not authenticated");
                return;
            }
            
            if (_viewModel.SelectedFilter == null)
            {
                MainWindowStore.Instance().DisplayError("Please select a filter");
                return;
            }
            
            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.GetFilterChanges(_viewModel.SelectedFilter.Id.FromHexString()).ContinueWith((t) =>
            {
                try
                {
                    // _viewModel.SelectedSolidityContract.SolidityContractAgg
                    var addFilterResult = t.Result;
                    var displayedStr = new StringBuilder();
                    if (addFilterResult != null && addFilterResult.Any())
                    {
                        foreach(var addFilter in addFilterResult)
                        {
                            var res = _viewModel.SelectedSolidityContract.SolidityContractAgg.GetLogs(addFilter.Topics.First().GetData(), addFilter.Data);
                            displayedStr.AppendLine(string.Format("{0} : {1}", res.Function.GetFullName(), string.Join(",", res.Data.Select(r => System.Text.Encoding.UTF8.GetString(r.ToArray())))));
                        }
                    }

                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayMessage(displayedStr.ToString()));
                }
                catch (AggregateException)
                {
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying get the last changes"));
                }
            });
        }

        private void ListenSmartContract(object sender, EventArgs e)
        {
            if (_viewModel == null) { return; }
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                MainWindowStore.Instance().DisplayError("You're not authenticated");
                return;
            }
            
            if (_viewModel.SelectedSolidityContract == null)
            {
                MainWindowStore.Instance().DisplayError("Contract must be selected");
                return;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.AddFilter(_viewModel.SelectedSolidityContract.Address.FromHexString()).ContinueWith((t) =>
            {
                try
                {
                    var addFilterResult = t.Result;
                    _solidityFilterRepository.Add(_viewModel.SelectedSolidityContract.Address, t.Result.ToHexString()).ContinueWith((s) =>
                    {
                        if (!s.Result)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MainWindowStore.Instance().DisplayMessage("The filter cannot be added");
                            });
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MainWindowStore.Instance().DisplayMessage("The filter has been added");
                            });
                        }
                    });
                }
                catch (AggregateException)
                {
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying add the filter"));
                }
            });
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

            if (_viewModel.SelectedSolidityContract == null)
            {
                MainWindowStore.Instance().DisplayError("Contract must be selected");
                return;
            }

            if (_viewModel.SelectedFunctionDefinition == null)
            {
                MainWindowStore.Instance().DisplayError("Function must be selected");
                return;
            }

            var callValue = _viewModel.SelectedFunctionDefinition.FunctionAgg.GetCallValue(_viewModel.SelectedFunctionDefinition.Parameters.Select(p => p.Value));
            var rpcClient = new RpcClient(authenticatedWallet.Network);
            var smartContractTransaction = new SmartContractTransaction
            {
                To = _viewModel.SelectedSolidityContract.Address.FromHexString(),
                Data = callValue.FromHexString()
            };
            rpcClient.SendRawTransaction(smartContractTransaction).ContinueWith((t) =>
            {
                try
                {
                    var txId = t.Result;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindowStore.Instance().DisplayMessage(string.Format("A new transaction has been published : {0}", txId));
                    });
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

            if (_viewModel.SelectedSolidityContract == null)
            {
                MainWindowStore.Instance().DisplayError("Contract must be selected");
                return;
            }

            if (_viewModel.SelectedFunctionDefinition == null)
            {
                MainWindowStore.Instance().DisplayError("Function must be selected");
                return;
            }

            var callValue = _viewModel.SelectedFunctionDefinition.FunctionAgg.GetCallValue(_viewModel.SelectedFunctionDefinition.Parameters.Select(p => p.Value));
            var smartContractTransactionParameter = new SmartContractTransactionParameter(_viewModel.SelectedSolidityContract.Address.FromHexString());
            smartContractTransactionParameter.Data = callValue.FromHexString();
            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.CallSmartContract(smartContractTransactionParameter).ContinueWith((t) =>
            {
                try
                {
                    var smartContractResult = t.Result;
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayMessage(string.Format("Result of the operation : {0}", smartContractResult)));
                }
                catch (AggregateException)
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
                    UpdateSmartContractDefinition(compilationResult);
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayMessage("The contract has been compiled"));
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
            _publishedSolidityContract = null;
            rpcClient.CompileSolidity(_viewModel.SmartContract).ContinueWith((t) =>
            {
                try
                {
                    var compilationResult = t.Result;
                    if (compilationResult.Infos == null || !compilationResult.Infos.Any())
                    {
                        return;
                    }

                    UpdateSmartContractDefinition(compilationResult);
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
                        _publishedSolidityContract = new SolidityContractAggregate
                        {
                            Abi = compilationResult.Infos.First().AbiDefinition.ToString(),
                            Code = compilationResult.Infos.First().Code
                        };
                    });
                }
                catch (AggregateException)
                {
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying to build the solidity contract"));
                }
            });
        }

        private void PersistSmartContract(object sender, EventArgs e)
        {
            if (_viewModel == null) { return; }
            var authenticatedWallet = WalletStore.Instance().GetAuthenticatedWallet();
            if (authenticatedWallet == null)
            {
                MainWindowStore.Instance().DisplayError("You're not authenticated");
                return;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.TransactionId))
            {
                MainWindowStore.Instance().DisplayError("The transaction address must be filled in");
                return;
            }

            if (_publishedSolidityContract == null)
            {
                MainWindowStore.Instance().DisplayError("The smart contract must be published");
                return;
            }

            IEnumerable<byte> txId = null;
            try
            {
                txId = _viewModel.TransactionId.FromHexString();
            }
            catch (Exception)
            {
                MainWindowStore.Instance().DisplayError("The transaction address is not a valid hex");
                return;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.GetTransactionReceipt(txId).ContinueWith((r) =>
            {
                try
                {
                    _publishedSolidityContract.Address = r.Result.ContractAddress;
                    _solidityContractsRepository.Insert(_publishedSolidityContract).ContinueWith((t) =>
                    {
                        try
                        {

                            if (t.Result)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    _viewModel.NewSmartContractAddress = r.Result.ContractAddress;
                                    _publishedSolidityContract = null;
                                    MainWindowStore.Instance().DisplayMessage("The transaction has been inserted");
                                });
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying to insert the smart contract"));
                            }
                        }
                        catch
                        {
                            Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying to insert the smart contract"));
                        }
                    });
                }
                catch (AggregateException)
                {
                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayError("An error occured while trying to insert the smart contract"));
                }
            });
        }

        private void Refresh(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => _viewModel.SmartContracts.Clear());
            _solidityContractsRepository.GetAll().ContinueWith((r) =>
            {
                foreach (var scAgg in r.Result)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var record = new SolidityContractViewModel
                        {
                            Address = scAgg.Address,
                            SolidityContractAgg = scAgg,
                            Filters = scAgg.Filters == null ? null : new ObservableCollection<FilterViewModel>(scAgg.Filters.Select(f => new FilterViewModel
                            {
                                Id = f
                            }))
                        };
                        var fns = scAgg.GetFunctions(SolidityFunctionTypes.FUNCTION);
                        foreach (var fn in fns)
                        {
                            record.Functions.Add(new FunctionDefinitionViewModel
                            {
                                FunctionName = fn.Name,
                                FunctionAgg = fn,
                                Parameters = new ObservableCollection<ParameterDefinitionViewModel>(fn.Parameters.Select(p => new ParameterDefinitionViewModel
                                {
                                    Name = p.Name,
                                    Type = p.Type
                                }).ToList())
                            });
                        }
                        _viewModel.SmartContracts.Add(record);
                    });
                }
            });
        }

        /*
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
                for (var i = 1; i <= selectedFnDef.Parameters.Count; i++)
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
        */

        private void UpdateSmartContractDefinition(CompileSolidityResponse compilationResult)
        {
            /*
            if (compilationResult.Infos == null)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() => _viewModel.FunctionDefinitions.Clear());
            foreach (var info in compilationResult.Infos)
            {
                var abiDefinition = info.AbiDefinition;
                foreach (JObject record in abiDefinition)
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
                            foreach (JObject inputDef in jArrInput)
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
            }
            */
        }
    }
}
