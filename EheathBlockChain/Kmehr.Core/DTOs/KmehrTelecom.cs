using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrTelecom
    {
        [XmlElement(Constants.KmehrTelecomNames.Cd)]
        public List<KmehrId> Cds { get; set; }
        [XmlElement(Constants.KmehrTelecomNames.TelecomNumber)]
        public string TelecomNumber { get; set; }
    }
}
