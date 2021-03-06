﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBlockChain.Core.Extensions;
using SimpleBlockChain.Core.Helpers;

namespace SimpleBlockChain.UnitTests.Helpers
{
    [TestClass]
    public class TargetHelperFixture
    {
        [TestMethod]
        public void WhenGetTarget()
        {
            uint nbits = 0x1d00ffff; // 4 bits     
            var result = TargetHelper.GetTarget(nbits);
            var hex = result.ToHexString();
            string s = "";
        }
    }
}
