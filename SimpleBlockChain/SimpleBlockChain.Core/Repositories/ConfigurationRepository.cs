using System;

namespace SimpleBlockChain.Core.Repositories
{
    public class ConfigurationRepository
    {
        public ConfigurationRepository()
        {

        }

        public UInt32 GetDefaultNBits()
        {
            return 0x1e0ffff0;
        }
    }
}
