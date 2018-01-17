using System.Collections.Generic;

namespace SimpleBlockChain.Data.Sqlite.Models
{
    public class SolidityContract
    {
        public string Address { get; set; }
        public string Code { get; set; }
        public string Abi { get; set; }
        public virtual ICollection<SolidityFilter> Filters { get; set; }
    }
}
