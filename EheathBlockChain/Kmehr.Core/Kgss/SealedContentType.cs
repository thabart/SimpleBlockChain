using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.Kgss
{
    [XmlRoot("SealedContentType")]
    public class SealedContentType
    {
        [XmlElement("SealedContent")]
        public IEnumerable<byte> SealedContent { get; set; }
    }
}
