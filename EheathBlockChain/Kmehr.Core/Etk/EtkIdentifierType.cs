using System.Xml.Serialization;

namespace Kmehr.Core.Etk
{
    public class EtkIdentifierType
    {
        [XmlElement("Type")]
        public string Type { get; set; }
        [XmlElement("Value")]
        public string Value { get; set; }
        [XmlElement("ApplicationID")]
        public string ApplicationId { get; set; }
    }
}
