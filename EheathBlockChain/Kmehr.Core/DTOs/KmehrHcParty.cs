using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    /// <summary>
    /// Reprents any kind of healthcare party : organization, physician, medical, speciality or even IT systems.
    /// </summary>
    public class KmehrHcParty
    {
        public KmehrHcParty()
        {

        }

        public KmehrHcParty(string hcPartyType)
        {
            Cd = new KmehrId(Constants.KmehrSenderQualifications.CDHCPARTY, "1.9", hcPartyType);
        }

        public KmehrHcParty(string hcPartyType, string id) : this(hcPartyType)
        {
            Id = new KmehrId(Constants.KmehrIdentifiers.IDHCPARTY, "1.0", id);
        }

        [XmlElement(Constants.KmehrHcPartyNames.Id)]
        public KmehrId Id { get; set; }
        [XmlElement(Constants.KmehrHcPartyNames.Cd)]
        public KmehrId Cd { get; set; }
        [XmlElement(Constants.KmehrHcPartyNames.Name)]
        public string Name { get; set; }
        [XmlElement(Constants.KmehrHcPartyNames.Firstname)]
        public string FirstName { get; set; }
        [XmlElement(Constants.KmehrHcPartyNames.Lastname)]
        public string Lastname { get; set; }
        [XmlElement(Constants.KmehrHcPartyNames.Telecom)]
        public List<KmehrTelecom> Telecoms { get; set; }
        [XmlElement(Constants.KmehrHcPartyNames.Address)]
        public KmehrAddress Address { get; set; }
    }
}
