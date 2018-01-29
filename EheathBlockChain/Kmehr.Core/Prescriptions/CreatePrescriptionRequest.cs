namespace Kmehr.Core.Requests
{
    /// <summary>
    /// Stores the prescription (kmehr format) in recip-e central system.
    /// Returns the RID (Recipe ID) to be printed on the paper.
    /// </summary>
    public class CreatePrescriptionRequest
    {
        public SecuredContentType SecuredCreatePrescriptionRequest { get; set; }
        public string AdministrativeInformation { get; set; }
    }
}
