using System.Collections.ObjectModel;

namespace SimpleBlockChain.WalletUI.ViewModels
{
    public class TxOutViewModel
    {
        public int Index { get; set; }
        public long Value { get; set; }
        public string Script { get; set; }
    }

    public class TransactionFlyoutViewModel : BaseViewModel
    {
        private string _txId;
        private uint _version;
        private string _previousTxId;
        private int _size;

        public TransactionFlyoutViewModel()
        {
            TxOuts = new ObservableCollection<TxOutViewModel>();
        }

        public string TxId
        {
            get
            {
                return _txId;
            }
            set
            {
                if (_txId != value)
                {
                    _txId = value;
                    NotifyPropertyChanged(nameof(TxId));
                }
            }
        }

        public uint Version
        {
            get
            {
                return _version;
            }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    NotifyPropertyChanged(nameof(Version));
                }
            }
        }

        public string PreviousTxId
        {
            get
            {
                return _previousTxId;
            }
            set
            {
                if (_previousTxId != value)
                {
                    _previousTxId = value;
                    NotifyPropertyChanged(nameof(PreviousTxId));
                }
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    NotifyPropertyChanged(nameof(Size));
                }
            }
        }

        public ObservableCollection<TxOutViewModel> TxOuts { get; set; }
    }
}
