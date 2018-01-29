using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.Kgss
{
    [XmlRoot("CredentialType")]
    public class CredentialType
    {
        [XmlElement("Namespace")]
        public string Namespace { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Value")]
        public List<string> Values { get; set; }
    }
}
