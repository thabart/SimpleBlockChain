﻿using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityFilter
    {
        public IEnumerable<byte> Id { get; set; }
        public IEnumerable<byte> SmartContractAddr { get; set; }
    }
}
