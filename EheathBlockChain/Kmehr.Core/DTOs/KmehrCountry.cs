using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrCountry
    {
        public KmehrCountry() { }

        public KmehrCountry(string country)
        {
            Cd = new KmehrId(Constants.KmehrSenderQualifications.CDFEDCOUNTRY, "1.2", country);
        }

        [XmlElement(Constants.KmehrCommonNames.Cd)]
        public KmehrId Cd { get; set; }
    }
}
