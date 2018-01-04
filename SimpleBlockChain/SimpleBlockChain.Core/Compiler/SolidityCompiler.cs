using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityCompiler
    {
        private static SolidityCompiler _instance;
        private Solc _solc;

        private SolidityCompiler()
        {
            _solc = new Solc();
        }

        public static void Compile(string contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            Instance().CompileSrc(contract);
        }

        public void CompileSrc(string contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            var fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".sol";
            File.Create(fileName).Close();
            File.AppendAllText(fileName, contract);
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = _solc.FilePath;
            processStartInfo.Arguments = string.Format("--bin \"{0}\"", fileName);
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
                        var binaries = GetBinaries(output);
                        bool b = true;
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
        }

        public static SolidityCompiler Instance()
        {
            if (_instance == null)
            {
                _instance = new SolidityCompiler();
            }

            return _instance;
        }

        private static IEnumerable<string> GetBinaries(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var cleanPattern = "\\r\\n [=]{7} [a-zA-Z0-9]+ [=]{7}\\r\\n";
            var splitPattern = "Binary: \\r\\n[a-zA-Z0-9]+\\r\\n";
            var cleanRgx = new Regex(cleanPattern);
            var splitRgx = new Regex(splitPattern);
            code = cleanRgx.Replace(code, "");
            var splittedValues = splitRgx.Matches(code);
            var binaries = new List<string>();
            foreach (Match splittedValue in splittedValues)
            {
                var binary = splittedValue.Value;
                binary = binary.Replace("Binary:", "");
                binary = binary.Replace("\r\n", "");
                binaries.Add(binary);
            }

            return binaries;
        }
    }
}
