using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityProgramPreCompile
    {
        private List<int> _jumpdest = new List<int>();

        public static SolidityProgramPreCompile Compile(byte[] ops)
        {
            var ret = new SolidityProgramPreCompile();
            var solidityOpCode = SolidityOpCode.Instance();
            for (int i = 0; i < ops.Length; ++i)
            {
                var op = solidityOpCode.GetCode(ops[i]);
                if (op == null) { continue; }
                if (op == SolidityOpCodes.JUMPDEST) { ret._jumpdest.Add(i); }
                if ((int)op >= (int)SolidityOpCodes.PUSH1 && (int)op <= (int) SolidityOpCodes.PUSH32)
                {
                    i += (int)op - (int)SolidityOpCodes.PUSH1 + 1;
                }
            }

            return ret;
        }

        public bool HasJumpDest(int pc)
        {
            return _jumpdest.Contains(pc);
        }
    }
}
