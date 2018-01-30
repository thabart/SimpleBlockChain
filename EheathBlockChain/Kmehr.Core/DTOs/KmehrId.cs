using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrId
    {
        [XmlAttribute(Constants.KmehrIdNames.S)]
        public string S { get; set; }
        [XmlAttribute(Constants.KmehrIdNames.Sv)]
        public string Sv { get; set; }
        [XmlAttribute(Constants.KmehrIdNames.Sl)]
        public string Sl { get; set; }
    }
}
