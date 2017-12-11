using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public enum ScriptTypes
    {
        P2PKH,
        P2SH
    }

    public enum ScriptRecordType
    {
        Operation,
        Stack
    }

    public class ScriptRecord
    {
        public ScriptRecord(OpCodes? opCode)
        {
            OpCode = opCode;
            Type = ScriptRecordType.Operation;
        }

        public ScriptRecord(IEnumerable<byte> stackRecord)
        {
            StackRecord = stackRecord;
            Type = ScriptRecordType.Stack;
        }

        public ScriptRecordType Type { get; private set; }
        public OpCodes? OpCode { get; private set; }
        public IEnumerable<byte> StackRecord { get; private set; }

        public IEnumerable<byte> Serialize()
        {
            var result = new List<byte>();
            switch(Type)
            {
                case ScriptRecordType.Operation:
                    result.AddRange(new byte[] { (byte)OpCode });
                    break;
                case ScriptRecordType.Stack:
                    result.AddRange(new byte[] { (byte)OpCodes.OP_DATA });
                    var compactSize = new CompactSize();
                    compactSize.Size = (ulong)StackRecord.Count();
                    result.AddRange(compactSize.Serialize());
                    result.AddRange(StackRecord);
                    break;
            }

            return result;
        }
    }

    public class Script
    {
        private static IEnumerable<OpCodes> _p2pkhOperations = new List<OpCodes>
        {
            OpCodes.OP_DUP,
            OpCodes.OP_HASH160,
            OpCodes.OP_EQUALVERIFY,
            OpCodes.OP_CHECKSIG
        };

        public Script(IEnumerable<ScriptRecord> scriptRecords)
        {
            ScriptRecords = scriptRecords;
        }

        public IEnumerable<ScriptRecord> ScriptRecords { get; private set; }
        public ScriptTypes? Type { get; private set; }

        public IEnumerable<byte> Serialize()
        {
            var result = new List<byte>();
            foreach(var scriptRecord in ScriptRecords)
            {
                result.AddRange(scriptRecord.Serialize());
            }

            return result;
        }

        public static Script CreateP2PKHScript(IEnumerable<byte> pubKeyHash)
        {
            var result = new List<ScriptRecord>();
            result.Add(new ScriptRecord(OpCodes.OP_DUP));
            result.Add(new ScriptRecord(OpCodes.OP_HASH160));
            result.Add(new ScriptRecord(pubKeyHash));
            result.Add(new ScriptRecord(OpCodes.OP_EQUALVERIFY));
            result.Add(new ScriptRecord(OpCodes.OP_CHECKSIG));
            return new Script(result);
        }

        public bool ContainsPublicKeyHash(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (ScriptRecords == null || ScriptRecords.Count() != 5 || !ScriptRecords.ElementAt(2).StackRecord.SequenceEqual(payload))
            {
                return false;
            }

            return true;
        }

        public static Script CreateCorrectScript()
        {
            var result = new List<ScriptRecord>();
            result.Add(new ScriptRecord(OpCodes.OP_15));
            result.Add(new ScriptRecord(OpCodes.OP_ADD));
            result.Add(new ScriptRecord(OpCodes.OP_16));
            result.Add(new ScriptRecord(OpCodes.OP_EQUAL));
            result.Add(new ScriptRecord(OpCodes.OP_VERIFY));
            return new Script(result);
        }

        public static Script Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                return null;
            }


            var scriptRecords = new List<ScriptRecord>();
            var opValues = Enum.GetValues(typeof(OpCodes)).Cast<byte>();
            int indice = 0;
            int nextIndice = 0;
            foreach (var b in payload)
            {
                if (!opValues.Contains(b) || indice < nextIndice)
                {
                    indice++;
                    continue;
                }

                var opCode = (OpCodes)b;
                if (opCode == OpCodes.OP_DATA)
                {
                    indice++;
                    var compactSize = CompactSize.Deserialize(payload.Skip(indice).ToArray());
                    var newIndice = indice + compactSize.Value;
                    var dataSize = (int)compactSize.Key.Size;
                    scriptRecords.Add(new ScriptRecord(payload.Skip(newIndice).Take(dataSize).ToList()));
                    nextIndice = newIndice + dataSize;
                    continue;
                }

                scriptRecords.Add(new ScriptRecord(opCode));
                indice++;
            }

            var opCodes = scriptRecords.Where(s => s.Type == ScriptRecordType.Operation).Select(s => s.OpCode.Value);
            var result = new Script(scriptRecords);
            if (opCodes.SequenceEqual(_p2pkhOperations))
            {
                result.Type = ScriptTypes.P2PKH;
            }

            return result;
        }
    }
}
