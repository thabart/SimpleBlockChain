using SimpleBlockChain.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Transactions
{
    public class TransactionOutSmartContract : BaseTransactionOut
    {
        public TransactionOutSmartContract(Script script, IEnumerable<byte> data, string author, string name) : base(script)
        {
            Data = data;
            Author = author;
            Name = name;
            Parameters = new string[0];
        }

        public TransactionOutSmartContract(Script script, IEnumerable<byte> data, string author, string name, IEnumerable<string> parameters) : base(script)
        {
            Data = data;
            Author = author;
            Name = name;
            Parameters = parameters;
        }

        public string Author { get; set; }
        public string Name { get; set; }
        public IEnumerable<byte> Data { get; set; }
        public IEnumerable<string> Parameters { get; set; }

        public override IEnumerable<byte> Serialize()
        {
            var result = new List<byte>();
            var scriptPayload = Script.Serialize();
            var dataPayload = Data;
            var authorPayload = System.Text.Encoding.UTF8.GetBytes(Author);
            var namePayload = System.Text.Encoding.UTF8.GetBytes(Name);
            var parametersPayload = System.Text.Encoding.UTF8.GetBytes(string.Join(",", Parameters));
            var compactSizeScript = new CompactSize();
            var compactSizeData = new CompactSize();
            var compactSizeAutor = new CompactSize();
            var compactSizeName = new CompactSize();
            var compactSizeParameters = new CompactSize();
            compactSizeScript.Size = (ulong)scriptPayload.Count();
            compactSizeData.Size = (ulong)dataPayload.Count();
            compactSizeAutor.Size = (ulong)authorPayload.Count();
            compactSizeName.Size = (ulong)namePayload.Count();
            compactSizeParameters.Size = (ulong)parametersPayload.Count();
            result.AddRange(compactSizeScript.Serialize()); // SCRIPT.
            result.AddRange(scriptPayload);
            result.AddRange(compactSizeData.Serialize()); // DATA.
            result.AddRange(dataPayload);
            result.AddRange(compactSizeAutor.Serialize()); // AUTHOR.
            result.AddRange(authorPayload);
            result.AddRange(compactSizeName.Serialize()); // NAME.
            result.AddRange(namePayload);
            result.AddRange(compactSizeParameters.Serialize()); // PARAMETERS.
            result.AddRange(parametersPayload);
            return result;
        }

        public static KeyValuePair<TransactionOutSmartContract, int> Deserialize(IEnumerable<byte> payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            int startIndex = 0;
            var compactSizeScript = CompactSize.Deserialize(payload.Skip(startIndex).ToArray()); // SCRIPT.
            startIndex += compactSizeScript.Value;
            var script = Script.Deserialize(payload.Skip(startIndex).Take((int)compactSizeScript.Key.Size));
            startIndex += (int)compactSizeScript.Key.Size;
            
            var compactSizeData = CompactSize.Deserialize(payload.Skip(startIndex).ToArray()); // AUTHOR.
            startIndex += compactSizeData.Value;
            var data = payload.Skip(startIndex).Take((int)compactSizeData.Key.Size);
            startIndex += (int)compactSizeData.Key.Size;

            var compactSizeAuthor = CompactSize.Deserialize(payload.Skip(startIndex).ToArray()); // AUTHOR.
            startIndex += compactSizeAuthor.Value;
            var author = System.Text.Encoding.UTF8.GetString(payload.Skip(startIndex).Take((int)compactSizeAuthor.Key.Size).ToArray());
            startIndex += (int)compactSizeAuthor.Key.Size;

            var compactSizeName = CompactSize.Deserialize(payload.Skip(startIndex).ToArray()); // AUTHOR.
            startIndex += compactSizeName.Value;
            var name = System.Text.Encoding.UTF8.GetString(payload.Skip(startIndex).Take((int)compactSizeName.Key.Size).ToArray());
            startIndex += (int)compactSizeName.Key.Size;

            var compactSizeParameters = CompactSize.Deserialize(payload.Skip(startIndex).ToArray()); // PARAMETERS.
            startIndex += compactSizeParameters.Value;
            var parameters = System.Text.Encoding.UTF8.GetString(payload.Skip(startIndex).Take((int)compactSizeParameters.Key.Size).ToArray()).Split(',');
            startIndex += (int)compactSizeParameters.Key.Size;

            return new KeyValuePair<TransactionOutSmartContract, int>(new TransactionOutSmartContract(script, data, author, name, parameters), startIndex);
        }
    }
}
