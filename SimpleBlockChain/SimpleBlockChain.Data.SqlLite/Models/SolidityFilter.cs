namespace SimpleBlockChain.Data.Sqlite.Models
{
    public class SolidityFilter
    {
        public string Id { get; set; }
        public string SmartContractAddress { get; set; }
        public virtual SolidityContract SmartContract { get; set; }
    }
}
