using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Rpc.Responses
{
    public class CompileSolidityResponseInfo
    {
        public string Code { get; set; }
        public JArray AbiDefinition { get; set; }
    }

    public class CompileSolidityResponse
    {
        public string Language { get; set; }
        public string LanguageVersion { get; set; }
        public string CompilerVersion { get; set; }
        public string Source { get; set; }
        public IEnumerable<CompileSolidityResponseInfo> Infos { get; set; }
    }
}
