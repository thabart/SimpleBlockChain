using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public enum KmehrHcPartyTypes
    {
        Orghospital,
        Persdentist,
        Persphysician,
        Persmidwife,
        Application
    }

    /// <summary>
    /// Reprents any kind of healthcare party : organization, physician, medical, speciality or even IT systems.
    /// </summary>
    public class KmehrHcParty
    {
        private Dictionary<KmehrHcPartyTypes, string> _mappingTypesToNames = new Dictionary<KmehrHcPartyTypes, string>
        {
            { KmehrHcPartyTypes.Orghospital, Constants.KmehrHcPartyTypesNames.Orghospital },
            { KmehrHcPartyTypes.Persdentist, Constants.KmehrHcPartyTypesNames.Persdentist },
            { KmehrHcPartyTypes.Persphysician, Constants.KmehrHcPartyTypesNames.Persphysician },
            { KmehrHcPartyTypes.Persmidwife, Constants.KmehrHcPartyTypesNames.Persmidwife },
            { KmehrHcPartyTypes.Application, Constants.KmehrHcPartyTypesNames.Application }
        };

        public KmehrHcParty()
        {

        }

        public KmehrHcParty(KmehrHcPartyTypes hcPartyType)
        {
            Cd = new KmehrId(Constants.KmehrSenderQualifications.CDHCPARTY, "1.9", _mappingTypesToNames[hcPartyType]);
        }

        public KmehrHcParty(KmehrHcPartyTypes hcPartyType, string id) : this(hcPartyType)
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
