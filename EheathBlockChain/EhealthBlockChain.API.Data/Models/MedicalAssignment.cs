namespace EhealthBlockChain.API.Data.Models
{
    internal class MedicalAssignment
    {
        public string MedicalBuildingId { get; set; }
        public string MedicalProviderId { get; set; }
        public virtual MedicalProvider MedicalProvider { get; set; }
        public virtual MedicalBuilding MedicalBuilding { get; set; }
    }
}
