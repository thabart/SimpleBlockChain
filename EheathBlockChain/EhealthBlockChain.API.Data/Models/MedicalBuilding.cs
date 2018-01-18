using System.Collections.Generic;

namespace EhealthBlockChain.API.Data.Models
{
    internal class MedicalBuilding
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<MedicalAssignment> MedicalAssignments { get; set; }
    }
}
