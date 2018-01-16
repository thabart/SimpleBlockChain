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
        }
        
        private void Load(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
            {
                _viewModel = new MemoryPoolInformationViewModel();
                DataContext = _viewModel;
                _viewModel.RefreshEvt += Refresh;
            }
        }

        private void Refresh(object sender, EventArgs e)
        {
            if (_viewModel == null) { return; }
            Init();
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
    }
}
