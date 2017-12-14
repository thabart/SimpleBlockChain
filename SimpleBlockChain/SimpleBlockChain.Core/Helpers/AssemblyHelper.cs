using System.Reflection;

namespace SimpleBlockChain.Core.Helpers
{
    public interface IAssemblyHelper
    {
        Assembly GetEntryAssembly();
    }

    internal class AssemblyHelper : IAssemblyHelper
    {
        public Assembly GetEntryAssembly()
        {
            return Assembly.GetEntryAssembly();
        }
    }
}
