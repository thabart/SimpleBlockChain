using HashLib;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Math;
using SimpleBlockChain.Core.Compiler;
using SimpleBlockChain.Core.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Aggregates
{
    public enum SolidityFunctionTypes
    {
        ALL,
        EVT,
        FUNCTION
    }

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

        public string GetFullName()
        {
            var operationSignature = string.Format("{0}({1})", Name, string.Join(",", Parameters.Select(p => p.Type)));
            return operationSignature;
        }

        public IEnumerable<byte> GetOperationHash()
        {
            var operationSignature = string.Format("{0}({1})", Name, string.Join(",", Parameters.Select(p => p.Type)));
            var hash = HashFactory.Crypto.SHA3.CreateKeccak256();
            return hash.ComputeBytes(System.Text.Encoding.ASCII.GetBytes(operationSignature)).GetBytes();
        }

        public IEnumerable<byte> GetOperationSignature()
        {
            return GetOperationHash().Take(4);
        }

        public string GetCallValue(IEnumerable<object> parameters)
        {
            var result = new List<byte>();
            var operationPayload = GetOperationSignature();
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

    public class FunctionResult
    {
        public SolidityContractAggregateFunction Function { get; set; }
        public List<IEnumerable<byte>> Data { get; set; }
    }

    public class SolidityContractAggregate
    {
        public string Address { get; set; }
        public string Code { get; set; }
        public string Abi { get; set; }
        public IEnumerable<string> Filters { get; set; }

        public FunctionResult GetLogs(IEnumerable<byte> topic, IEnumerable<byte> data)
        {
            var eventDefs = GetFunctions(SolidityFunctionTypes.EVT);
            var topicHex = topic.ToHexString();
            var evtDef = eventDefs.FirstOrDefault(edef => edef.GetOperationHash().ToHexString() == topicHex);
            if (evtDef == null)
            {
                return null;
            }

            int offset = 0;
            var result = new List<IEnumerable<byte>>();
            foreach (var parameter in evtDef.Parameters)
            {
                if (parameter.Type == "string" || parameter.Type == "bytes")
                {
                    var offsetParameter = new BigInteger(data.Skip(offset).Take(32).ToArray()).IntValue;
                    var parameterSize = new BigInteger(data.Skip(offsetParameter).Take(32).ToArray()).IntValue;
                    result.Add(data.Skip(offsetParameter + 32).Take(parameterSize));
                    offset += 32;
                }
                else
                {
                    result.Add(data.Skip(offset).Take(32));
                    offset += 32;
                }
            }

            return new FunctionResult
            {
                Function = evtDef,
                Data = result
            };
        }

        public IEnumerable<SolidityContractAggregateFunction> GetFunctions(SolidityFunctionTypes type)
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
                if (type == SolidityFunctionTypes.EVT && fnType != "event")
                {
                    continue;
                }

                if (type == SolidityFunctionTypes.FUNCTION && fnType != "function")
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
