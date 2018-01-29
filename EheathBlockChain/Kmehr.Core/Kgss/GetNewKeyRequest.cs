using System.Xml.Serialization;

namespace Kmehr.Core.Kgss
{
    [XmlRoot("GetNewKeyRequest")]
    public class GetNewKeyRequest
    {
        [XmlElement("SealedNewKeyRequest")]
        public SealedContentType SealedNewKeyRequest { get; set; }
    }
}
