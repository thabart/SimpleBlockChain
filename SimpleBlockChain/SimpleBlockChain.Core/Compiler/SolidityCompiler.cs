using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityCompilerResult
    {
        public string Contract { get; set; }
        public string Payload { get; set; }
        public JArray AbiCode { get; set; }
    }

    public class SolidityCompiler
    {
        private static SolidityCompiler _instance;
        private Solc _solc;

        private SolidityCompiler()
        {
            _solc = new Solc();
        }

        public static IEnumerable<JArray> GetAbi(string contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }
            
            return Instance().GetAbiSrc(contract);
        }

        public static IEnumerable<SolidityCompilerResult> Compile(string contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            return Instance().CompileSrc(contract);
        }

        public IEnumerable<SolidityCompilerResult> CompileSrc(string contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            IEnumerable<SolidityCompilerResult> result = new List<SolidityCompilerResult>();
            var fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".sol";
            File.Create(fileName).Close();
            File.AppendAllText(fileName, contract);
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = _solc.FilePath;
            processStartInfo.Arguments = string.Format("--bin --abi \"{0}\"", fileName);
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            using (var process = Process.Start(processStartInfo))
            {
                using (StreamReader standardOutput = process.StandardOutput)
                {
                    var output = standardOutput.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        result = ParseBinariesAndAbi(output);
                    }
                }

                if (!result.Any())
                {
                    using (StreamReader standardError = process.StandardError)
                    {
                        string error = standardError.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(error))
                        {
                            throw new InvalidOperationException(error);
                        }
                    }
                }

                process.WaitForExit();
            }

            File.Delete(fileName);
            return result;
        }

        public IEnumerable<JArray> GetAbiSrc(string contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            IEnumerable<JArray> result = new List<JArray>();
            var fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".sol";
            File.Create(fileName).Close();
            File.AppendAllText(fileName, contract);
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = _solc.FilePath;
            processStartInfo.Arguments = string.Format("--abi \"{0}\"", fileName);
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            using (var process = Process.Start(processStartInfo))
            {
                using (StreamReader standardOutput = process.StandardOutput)
                {
                    var output = standardOutput.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        result = ParseAbi(output);
                    }
                }
                using (StreamReader standardError = process.StandardError)
                {
                    string error = standardError.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        throw new InvalidOperationException(error);
                    }
                }

                process.WaitForExit();
            }

            File.Delete(fileName);
            return result;
        }

        public static SolidityCompiler Instance()
        {
            if (_instance == null)
            {
                _instance = new SolidityCompiler();
            }

            return _instance;
        }

        private static IEnumerable<SolidityCompilerResult> ParseBinariesAndAbi(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var binarySplitPattern = "Binary: \\r\\n[0-9a-zA-Z]*";
            var binarySplitRegex = new Regex(binarySplitPattern);
            var splittedBinaries = binarySplitRegex.Matches(code);
            var binaries = new List<string>();
            foreach (Match splittedValue in splittedBinaries)
            {
                var binary = splittedValue.Value;
                binary = binary.Replace("Binary:", "");
                binary = binary.Replace("\r\n", "");
                binary = binary.Replace(" ", "");
                binaries.Add(binary);
            }

            var abiLst = ParseAbi(code);
            var result = new List<SolidityCompilerResult>();
            for(var i = 0; i < binaries.Count; i++)
            {
                var record = new SolidityCompilerResult
                {
                    Contract = code,
                    AbiCode = abiLst.ElementAt(i),
                    Payload = binaries.ElementAt(i)
                };
                result.Add(record);
            }

            return result;
        }

        private static IEnumerable<JArray> ParseAbi(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var pattern = "\\[\\S*\\]";
            var splitRgx = new Regex(pattern);
            var splittedValues = splitRgx.Matches(code);
            var abisJson = new List<JArray>();
            foreach (Match splittedValue in splittedValues)
            {
                var abi = splittedValue.Value;
                abisJson.Add(JArray.Parse(abi));
            }

            return abisJson;
        }
    }
}
