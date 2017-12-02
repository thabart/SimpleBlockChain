using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;
using Xunit;

namespace SimpleBlockChain.UnitTests.Helpers
{
    public class TargetHelperFixture
    {
        [Fact]
        public void WhenGetTarget()
        {
            uint nbits = 0x1d00ffff; // 4 bits     
            var result = TargetHelper.GetTarget(nbits);
            var hex = result.ToHexString();
            string s = "";
        }
    }
}
