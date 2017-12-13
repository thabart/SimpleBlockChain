using System.Collections.ObjectModel;

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
        }

        public ObservableCollection<BlockTransactionViewModel> Transactions { get; set; }
        public TransactionViewModel SelectedTransaction { get; set; }
    }
}
