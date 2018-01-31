namespace Kmehr.EF.Models
{
    internal class Translation
    {
        public string LanguageId { get; set; }
        public string HealthCarePartyTypeId { get; set; }
        public string Value { get; set; }
        public virtual Language Language { get; set; }
        public virtual HealthCarePartyType HealthCarePartyType { get; set; }
    }
}
