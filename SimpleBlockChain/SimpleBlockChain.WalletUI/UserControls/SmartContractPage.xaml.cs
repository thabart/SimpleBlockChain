using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Rpc.Parameters;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.WalletUI.Stores;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            _viewModel = null;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            _viewModel = new SmartContractViewModel();
            _viewModel.CallContractEvt += CallContract;
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
                Trace.WriteLine(t);
                string s;
            });
        }
    }
}
