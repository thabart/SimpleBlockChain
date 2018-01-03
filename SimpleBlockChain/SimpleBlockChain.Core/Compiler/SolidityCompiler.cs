using System;
using System.Diagnostics;
using System.IO;

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
            var process = new Process();
            process.StartInfo.FileName = _solc.FilePath;
            process.StartInfo.Arguments = "--bin " + fileName;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            string s = "";
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            string s = "";
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string s = "";
        }

        public static SolidityCompiler Instance()
        {
            if (_instance == null)
            {
                _instance = new SolidityCompiler();
            }

            return _instance;
        }
    }
}
