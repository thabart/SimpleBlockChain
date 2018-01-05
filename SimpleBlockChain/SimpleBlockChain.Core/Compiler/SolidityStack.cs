using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityStack : List<DataWord>
    {
        public DataWord Pop()
        {
            var result = (DataWord)this.Last().Clone();
            RemoveAt(Count - 1);
            return result;
        }

        public void Swap(int from, int to)
        {
            if (IsAccessible(from) && IsAccessible(to) && (from != to))
            {
                DataWord tmp = this.ElementAt(from);
                this[from] = this[to];
                this[to] = tmp;
            }
        }

        private bool IsAccessible(int from)
        {
            return from >= 0 && from < this.Count();
        }
    }
}
