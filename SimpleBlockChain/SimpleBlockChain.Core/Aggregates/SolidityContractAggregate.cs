using HashLib;
using Newtonsoft.Json.Linq;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Aggregates
{
    public class SolidityContractAggregateParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class SolidityContractAggregateFunction
    {
        public SolidityContractAggregateFunction()
        {
            Parameters = new List<SolidityContractAggregateParameter>();
        }

        public string Name { get; set; }
        public IEnumerable<SolidityContractAggregateParameter> Parameters { get; set; }

        public string GetCallValue(IEnumerable<object> parameters)
        {
            var result = new List<byte>();
            var operationSignature = string.Format("{0}({1})", Name, string.Join(",", Parameters.Select(p => p.Type)));
            var hash = HashFactory.Crypto.SHA3.CreateKeccak256();
            var operationPayload = hash.ComputeBytes(System.Text.Encoding.ASCII.GetBytes(operationSignature)).GetBytes().Take(4);
            result.AddRange(operationPayload);
            var indice = 0;
            var dynamicResult = new List<byte>();
            foreach (var parameterDef in Parameters)
            {
                if (parameterDef.Type == "string" || parameterDef.Type == "bytes") // Complex type.
                {
                    result.AddRange(new DataWord(32 * (Parameters.Count() + indice)).GetData());
                    dynamicResult.AddRange(new DataWord(System.Text.Encoding.ASCII.GetBytes(parameters.ElementAt(indice).ToString()).Length).GetData());
                    dynamicResult.AddRange(new DataWord(System.Text.Encoding.ASCII.GetBytes(parameters.ElementAt(indice).ToString())).GetReverseData());
                }
                else
                {
                    result.AddRange(new DataWord(int.Parse(parameters.ElementAt(indice).ToString())).GetData()); // TODO : SUPPORT ONLY INT.
                }

                indice++;
            }

            result.AddRange(dynamicResult);
            return result.ToHexString();
        }
    }

    public class SolidityContractAggregate
    {
        public string Address { get; set; }
        public string Code { get; set; }
        public string Abi { get; set; }

        public IEnumerable<SolidityContractAggregateFunction> GetFunctions()
        {
            var result = new List<SolidityContractAggregateFunction>();
            JArray jArr = JArray.Parse(Abi);
            foreach (JObject record in jArr)
            {
                JToken jTokenName = null;
                if (!record.TryGetValue("name", out jTokenName))
                {
                    continue;
                }

                JToken jTokenInputs = null;
                var fnType = record.GetValue("type").ToString();
                if (fnType != "function")
                {
                    continue;
                }

                var parameters = new List<SolidityContractAggregateParameter>();
                if (record.TryGetValue("inputs", out jTokenInputs))
                {
                    var jArrInput = jTokenInputs as JArray;
                    if (jArrInput != null)
                    {
                        foreach (JObject inputDef in jArrInput)
                        {
                            parameters.Add(new SolidityContractAggregateParameter
                            {
                                Name = inputDef.GetValue("name").ToString(),
                                Type = inputDef.GetValue("type").ToString()
                            });
                        }
                    }
                }

                var newFuncDef = new SolidityContractAggregateFunction
                {
                    Name = jTokenName.ToString(),
                    Parameters = parameters
                };
                result.Add(newFuncDef);
            }

            return result;
        }
    }
}
