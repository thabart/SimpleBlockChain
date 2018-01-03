using System.IO;
using System.Reflection;

namespace SimpleBlockChain.Core.Compiler
{
    public class Solc
    {
        private static string _fileName = "solc.exe";

        public Solc()
        {
            Init();
        }

        public string FilePath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _fileName);
            }
        }

        private void Init()
        {
            var filePath = FilePath;
            if (File.Exists(filePath))
            {
                return;
            }

            var ass = Assembly.GetExecutingAssembly();
            var names = ass.GetManifestResourceNames();
            var stream = ass.GetManifestResourceStream("SimpleBlockChain.Core.Native.Windows.solc.exe");
            if (stream == null)
            {
                // TODO : Throw an exception.
            }

            using (var output = File.Create(filePath))
            {
                stream.CopyTo(output);
            }
        }
    }
}
