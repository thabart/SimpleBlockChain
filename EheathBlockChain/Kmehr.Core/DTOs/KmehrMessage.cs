using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrMessage
    {
        [XmlElement(Constants.KmehrMessageNames.Header)]
        public KmehrHeader Header { get; set; }
        [XmlElement(Constants.KmehrMessageNames.Folder)]
        public KmehrFolder Fodler { get; set; }
    }
}
