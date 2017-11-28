using SimpleBlockChain.Core.Crypto;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Scripts
{
    public class Interpreter
    {
        private Dictionary<OpCodes, IEnumerable<byte>> _mappingOpCodesToBytes = new Dictionary<OpCodes, IEnumerable<byte>>
        {
            { OpCodes.OP_1, BitConverter.GetBytes(1) },
            { OpCodes.OP_2, BitConverter.GetBytes(2) },
            { OpCodes.OP_3, BitConverter.GetBytes(3) },
            { OpCodes.OP_4, BitConverter.GetBytes(4) },
            { OpCodes.OP_5, BitConverter.GetBytes(5) },
            { OpCodes.OP_6, BitConverter.GetBytes(6) },
            { OpCodes.OP_7, BitConverter.GetBytes(7) },
            { OpCodes.OP_8, BitConverter.GetBytes(8) },
            { OpCodes.OP_9, BitConverter.GetBytes(9) },
            { OpCodes.OP_10, BitConverter.GetBytes(10) },
            { OpCodes.OP_11, BitConverter.GetBytes(11) },
            { OpCodes.OP_12, BitConverter.GetBytes(12) },
            { OpCodes.OP_13, BitConverter.GetBytes(13) },
            { OpCodes.OP_14, BitConverter.GetBytes(14) },
            { OpCodes.OP_15, BitConverter.GetBytes(15) },
            { OpCodes.OP_16, BitConverter.GetBytes(16) }
        };

        public bool Check(Script firstScript, Script secondScript)
        {
            var stack = new List<IEnumerable<byte>>();
            if (firstScript == null)
            {
                throw new ArgumentNullException(nameof(firstScript));
            }

            if (secondScript == null)
            {
                throw new ArgumentNullException(nameof(secondScript));
            }

            var concatenatedScripts = new List<ScriptRecord>();
            concatenatedScripts.AddRange(firstScript.ScriptRecords);
            concatenatedScripts.AddRange(secondScript.ScriptRecords);
            var newScript = new Script(concatenatedScripts);
            foreach(var scriptRecord in newScript.ScriptRecords)
            {
                if (scriptRecord.Type == ScriptRecordType.Stack)
                {
                    stack.Add(scriptRecord.StackRecord);
                    continue;
                }

                if (_mappingOpCodesToBytes.ContainsKey(scriptRecord.OpCode.Value))
                {
                    stack.Add(_mappingOpCodesToBytes[scriptRecord.OpCode.Value]);
                    continue;
                }

                var ra = stack.ElementAt(stack.Count() - 2);
                var rb = stack.Last();
                var a = ra.ToArray();
                var b = rb.ToArray();
                var ba = new BigInteger(a);
                var bb = new BigInteger(b);
                switch (scriptRecord.OpCode)
                {
                    case OpCodes.OP_ADD:
                        var sum = ba + bb;
                        stack.Add(sum.ToByteArray());
                        break;
                    case OpCodes.OP_EQUAL:
                        stack.Remove(stack.Last());
                        stack.Remove(stack.Last());
                        var pop = (ba == bb) ? new byte[] { 1 } : new byte[] { 0 };
                        stack.Add(pop);
                        break;
                    case OpCodes.OP_VERIFY:
                        if (bb == 0) { return false; }
                        break;
                    case OpCodes.OP_EQUALVERIFY:
                        stack.Remove(stack.Last());
                        stack.Remove(stack.Last());
                        if (ba != bb) { return false; }
                        break;
                    case OpCodes.OP_DUP:
                        stack.Add(b);
                        break;
                    case OpCodes.OP_HASH160:
                        var myRIPEMD160 = RIPEMD160.Create();
                        var mySHA256 = SHA256.Create();
                        var n = myRIPEMD160.ComputeHash(mySHA256.ComputeHash(b));
                        stack.Remove(stack.Last());
                        stack.Add(n);
                        break;
                    case OpCodes.OP_CHECKSIG:
                        var sig = ra;
                        var publicKey = rb;
                        var key = Key.Deserialize(publicKey);
                        var payload = System.Text.Encoding.UTF8.GetBytes(Constants.DEFAULT_SIGNATURE_CONTENT);
                        var isCorrect = key.CheckSignature(payload, sig);
                        var p = (isCorrect) ? new byte[] { 1 } : new byte[] { 0 };
                        stack.Add(p);
                        break;
                }
            }

            return new BigInteger(stack.Last().ToArray()) == 1;
        }
    }
}
