using SimpleBlockChain.Core;
using SimpleBlockChain.Core.Rpc;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.WalletUI.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlockChain.WalletUI.UserControls
{
    public partial class MemoryPoolInformation : UserControl
    {
        private MemoryPoolInformationViewModel _viewModel;

        public MemoryPoolInformation()
        {
            InitializeComponent();
            Loaded += Load;
            Unloaded += Unload;
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            Destroy();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            _viewModel = new MemoryPoolInformationViewModel();
            DataContext = _viewModel;
            var walletStore = WalletStore.Instance();
            var rpcClient = new RpcClient(walletStore.GetAuthenticatedWallet().Network);
            rpcClient.GetRawMemPool(true).ContinueWith((r) =>
            {
                try
                {
                    var result = r.Result;
                    foreach(var rawMemPool in result)
                    {
                        var record = new RawMemPoolViewModel
                        {
                            Fee = rawMemPool.Fee,
                            Time = rawMemPool.Time,
                            TxId = rawMemPool.TxId
                        };
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _viewModel.Raws.Add(record);
                        });
                    }
                }
                catch(AggregateException ex)
                {

                }
            });
        }

        private void Destroy()
        {
            _viewModel = null;
        }
    }
}
