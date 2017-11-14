using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Transactions
{
    public class ScriptBuilder
    {
        private List<byte> _payload { get; set; }
        
        public ScriptBuilder()
        {
            _payload = new List<byte>();
        }

        public void New()
        {
            _payload = new List<byte>();
        }

        public void AddOperation(OpCodes code)
        {
            _payload.AddRange(BitConverter.GetBytes((uint)code));
        }

        public void AddData(IEnumerable<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _payload.AddRange(data);
        }

        public Script Create()
        {
            return new Script(_payload);
        }

        public Script CreateP2PKHScript(IEnumerable<byte> publicKeyHash)
        {
            if (publicKeyHash == null)
            {
                throw new ArgumentNullException(nameof(publicKeyHash));
            }

            New();
            AddOperation(OpCodes.OP_DUP);
            AddOperation(OpCodes.OP_HASH160);
            AddData(publicKeyHash);
            AddOperation(OpCodes.OP_EQUALVERIFY);
            AddOperation(OpCodes.OP_CHECKSIG);
            return Create();
        }
    }
}
