using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public enum KmehrAddressTypes
    {
        Work
    }

    public class KmehrAddress
    {
        private static Dictionary<KmehrAddressTypes, string> _mapping = new Dictionary<KmehrAddressTypes, string>
        {
            { KmehrAddressTypes.Work, Constants.KmehrAddressTypeNames.Work }
        };

        public KmehrAddress() { }

        public KmehrAddress(KmehrAddressTypes type)
        {
            Cd = new KmehrId(Constants.KmehrSenderQualifications.CDADDRESS, "1.1", _mapping[type]);
        }

        [XmlElement(Constants.KmehrCommonNames.Cd)]
        public KmehrId Cd { get; set; }
        [XmlElement(Constants.KmehrAddressNames.Country)]
        public KmehrCountry Country { get; set; }
        [XmlElement(Constants.KmehrAddressNames.Zip)]
        public string Zip { get; set; }
        [XmlElement(Constants.KmehrAddressNames.City)]
        public string City { get; set; }
        [XmlElement(Constants.KmehrAddressNames.Street)]
        public string Street { get; set; }
        [XmlElement(Constants.KmehrAddressNames.HouseNumber)]
        public string HouseNumber { get; set; }
    }
}
