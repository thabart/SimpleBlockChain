using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Builders
{
    public class ScriptBuilder
    {
        private IList<ScriptRecord> _scriptRecords;

        public ScriptBuilder New()
        {
            _scriptRecords = new List<ScriptRecord>();
            return this;
        }

        public ScriptBuilder AddToStack(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            _scriptRecords.Add(new ScriptRecord(payload));
            return this;
        }

        public ScriptBuilder AddOperation(OpCodes opCode)
        {
            _scriptRecords.Add(new ScriptRecord(opCode));
            return this;
        }

        public Script Build()
        {
            return new Script(_scriptRecords);
        }
    }
}
