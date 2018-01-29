using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.Kgss
{
    [XmlRoot("GetKeyRequestContent")]
    public class GetKeyRequestContent
    {
        [XmlElement("KeyIdentifier")]
        public IEnumerable<byte> KeyIdentifier { get; set; }
        [XmlElement("KeyEncryptionKey")]
        public IEnumerable<byte> KeyEncryptionKey { get; set; }
        [XmlElement("KeyEncryptionKeyIdentifier")]
        public IEnumerable<byte> KeyEncryptionKeyIdentifier { get; set; }
        [XmlElement("ETK")]
        public IEnumerable<byte> Etk { get; set; }
    }
}
