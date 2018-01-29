using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.Kgss
{
    [XmlRoot("GetKeyRequestContent")]
    public class GetNewKeyRequestContent
    {
        [XmlElement("AllowedReader")]
        public List<CredentialType> AllowedReaders { get; set; }
        [XmlElement("ExludedReader")]
        public List<CredentialType> ExludedReaders { get; set; }
        [XmlElement("ETK")]
        public byte[] Etk { get; set; }
    }
}
