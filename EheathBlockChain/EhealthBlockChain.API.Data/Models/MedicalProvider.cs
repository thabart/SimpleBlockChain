using System.Collections.Generic;

namespace EhealthBlockChain.API.Data.Models
{
    internal class MedicalProvider
    {
        public string InamiCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<MedicalAssignment> MedicalAssignments { get; set; }
    }
}
