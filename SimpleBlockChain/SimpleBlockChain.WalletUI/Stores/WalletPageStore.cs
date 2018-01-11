namespace SimpleBlockChain.WalletUI.Stores
{
    internal class WalletPageStore
    {
        private static WalletPageStore _instance;

        public static WalletPageStore Instance()
        {
            if (_instance == null)
            {
                _instance = new WalletPageStore();
            }

            return _instance;
        }
        
        public int NbBlocks { get; set; }
    }
}
