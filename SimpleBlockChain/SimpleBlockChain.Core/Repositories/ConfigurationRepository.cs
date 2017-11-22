namespace SimpleBlockChain.Core.Repositories
{
    public class ConfigurationRepository
    {
        public ConfigurationRepository()
        {

        }

        public uint GetDefaultNBits()
        {
            return Constants.DEFAULT_NBITS;
        }
    }
}
