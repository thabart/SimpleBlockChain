namespace SimpleBlockChain.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var p2pNetworkConnector = new P2PNetworkConnector(Core.Networks.TestNet);
            p2pNetworkConnector.Listen();
        }
    }
}
