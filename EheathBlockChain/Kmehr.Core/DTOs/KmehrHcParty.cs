using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrHcParty
    {
        [XmlElement(Constants.KmehrHcPartyNames.Id)]
        public KmehrId Id { get; set; }
        [XmlElement(Constants.KmehrHcPartyNames.Cd)]
        public KmehrId Cd { get; set; }
        [XmlElement(Constants.KmehrHcPartyNames.Name)]
        public string Name { get; set; }
        [XmlElement(Constants.KmehrHcPartyNames.Telecom)]
        public List<KmehrTelecom> Telecoms { get; set; }
    }
}
