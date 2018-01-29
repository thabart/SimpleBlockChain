using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.Requests
{
    [XmlRoot("CreatePrescriptionAdministrativeInformationType")]
    public class CreatePrescriptionAdministrativeInformationType
    {
        [XmlElement("PrescriberIdentifier", Order = 1)]
        public IdentifierType PrescriberIdentifier { get; set; }
        [XmlElement("PrescriptionType", Order = 2)]
        public string PrescriptionType { get; set; }
        [XmlElement("KeyIdentifier", Order = 3)]
        public IEnumerable<byte> KeyIdentifier { get; set; }
    }
}
