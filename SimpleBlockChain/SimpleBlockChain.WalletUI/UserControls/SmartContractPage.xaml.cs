﻿using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Rpc.Parameters;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.WalletUI.Stores;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.UserControls
{
    public partial class SmartContractPage : UserControl
    {
        private SmartContractViewModel _viewModel;

        public SmartContractPage()
        {
            InitializeComponent();
            Loaded += Load;
            Unloaded += Unload;
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) { return; }
            _viewModel.CallContractEvt -= CallContract;
            _viewModel.CompileContractEvt -= CompileContract;
            _viewModel.PublishContractEvt -= PublishContract;
            _viewModel = null;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            _viewModel = new SmartContractViewModel();
            _viewModel.CallContractEvt += CallContract;
            _viewModel.CompileContractEvt += CompileContract;
            _viewModel.PublishContractEvt += PublishContract;
            DataContext = _viewModel;
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
                MainWindowStore.Instance().DisplayError("The solidity contract should be filled in");
                return;
            }

            var rpcClient = new RpcClient(authenticatedWallet.Network);
            rpcClient.CompileSolidity(_viewModel.SmartContract).ContinueWith((t) =>
            {
                try
                {
                    var compilationResult = t.Result;
                    var builder = new StringBuilder();
                    builder.Append("BUILD RESULT");
                    builder.AppendLine();
                    foreach(var info in compilationResult.Infos)
                    {
                        builder.Append("Code: " + info.Code);
                        builder.AppendLine();
                        builder.Append("ABI : " + info.AbiDefinition.ToString());
                        builder.AppendLine();
                    }

                    Application.Current.Dispatcher.Invoke(() => MainWindowStore.Instance().DisplayMessage(builder.ToString()));
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


            throw new NotImplementedException();
        }
    }
}
