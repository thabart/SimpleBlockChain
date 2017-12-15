using SimpleBlockChain.WalletUI.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class BlockTransactionViewModel
    {
        public string TxId { get; set; }
        public bool IsCoinBase { get; set; }
        public long Value { get; set; }
        public int NbTransactionsOut { get; set; }
    }

    public class BlockFlyoutViewModel
    {
        public BlockFlyoutViewModel()
        {
            Transactions = new ObservableCollection<BlockTransactionViewModel>();
            OpenTransactionCommand = new RelayCommand(p => OpenTransaction(), p => CanOpenTransaction());
        }

        public ObservableCollection<BlockTransactionViewModel> Transactions { get; set; }
        public BlockTransactionViewModel SelectedTransaction { get; set; }
        public ICommand OpenTransactionCommand { get; set; }
        public event EventHandler OpenTransactionEvt;

        private void OpenTransaction()
        {
            if (SelectedTransaction == null)
            {
                return;
            }

            if (OpenTransactionEvt != null)
            {
                OpenTransactionEvt(this, EventArgs.Empty);
            }
        }

        private bool CanOpenTransaction()
        {
            return true;
        }
    }
}
