using Org.BouncyCastle.Math;
using SimpleBlockChain.Core.Crypto;

namespace SimpleBlockChain.UnitTests.Stores
{
    public static class KeyStore
    {
        public static Key GetGenesisKey()
        {
            return Key.Deserialize(new BigInteger("66661394595692466950200829442443674598224300882267065208709422638481412972116609477112206002430829808784107536250360432119209033266013484787698545014625057"), new BigInteger("43102461949956883352376427470284148089747996528740865531180015053863743793176"));
        }
    }
}
