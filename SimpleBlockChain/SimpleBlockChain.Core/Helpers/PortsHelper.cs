namespace SimpleBlockChain.Core.Helpers
{
    public static class PortsHelper
    {
        public static string GetPort(Networks network)
        {
            switch(network)
            {
                case Networks.MainNet:
                    return Constants.Ports.MainNet;
                case Networks.TestNet:
                    return Constants.Ports.TestNet;
                case Networks.RegTest:
                    return Constants.Ports.RegTest;
            }

            return null;
        }

        public static string GetRPCPort(Networks network)
        {
            switch(network)
            {
                case Networks.MainNet:
                    return Constants.RpcPorts.MainNet;
                case Networks.RegTest:
                    return Constants.RpcPorts.RegTest;
                case Networks.TestNet:
                    return Constants.RpcPorts.TestNet;
            }

            return null;
        }
    }
}
