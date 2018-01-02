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

        private void Load(object sender, RoutedEventArgs e)
        {
            _viewModel = new MemoryPoolInformationViewModel();
            DataContext = _viewModel;
        }

        public void Refresh()
        {
            if (_viewModel == null) { return; }
            Init();
        }

        public void Reset()
        {
            if (_viewModel == null) { return; }
            _viewModel.Reset();
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            Destroy();
        }

        private void Init()
        {
            var walletStore = WalletStore.Instance();
            var rpcClient = new RpcClient(walletStore.GetAuthenticatedWallet().Network);
            Application.Current.Dispatcher.Invoke(() => {
                _viewModel.Raws.Clear();
            });
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
                            TxId = rawMemPool.TxId,
                            AncestorCount = rawMemPool.AncestorCount,
                            DescendantCount = rawMemPool.DescendantCount
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
