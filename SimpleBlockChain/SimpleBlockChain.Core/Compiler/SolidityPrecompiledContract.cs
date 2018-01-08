using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public abstract class SolidityPrecompiledContract
    {
        private static DataWord _identityAddr = new DataWord("0000000000000000000000000000000000000000000000000000000000000004");
        private static SolidityIdentity _identity = new SolidityIdentity();

        public static SolidityPrecompiledContract GetContractForAddress(DataWord address)
        {
            if (address == null) return _identity;
            if (address.Equals(_identityAddr)) return _identity;
            return null;
        }

        public abstract long GetGasForData(byte[] data);
        public abstract KeyValuePair<bool, byte[]> Execute(byte[] data);

        public class SolidityIdentity : SolidityPrecompiledContract
        {
            public SolidityIdentity()
            {
            }

            public override long GetGasForData(byte[] data)
            {
                if (data == null) return 15;
                return 15 + (data.Length + 31) / 32 * 3;
            }


            public override KeyValuePair<bool, byte[]> Execute(byte[] data)
            {
                return new KeyValuePair<bool, byte[]>(true, data);
            }
        }
    }
}
