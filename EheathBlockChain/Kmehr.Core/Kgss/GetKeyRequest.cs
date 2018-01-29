using System.Xml.Serialization;

namespace Kmehr.Core.Kgss
{
    public class GetKeyRequest
    {
        [XmlElement("SealedContentType")]
        public SealedContentType SealedKeyRequest { get; set; }
    }
}
