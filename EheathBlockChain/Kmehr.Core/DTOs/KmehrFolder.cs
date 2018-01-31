using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrFolder
    {
        public KmehrFolder()
        {
            Id = new KmehrId(Constants.KmehrIdentifiers.IDKMEHR, "1.0", "1");
        }

        [XmlElement(Constants.KmehrFolderNames.Id)]
        public KmehrId Id { get; set; }
        [XmlElement(Constants.KmehrFolderNames.Patient)]
        public KmehrPatient Patient { get; set; }
        [XmlElement(Constants.KmehrFolderNames.Transaction)]
        public KmehrTransaction Transaction { get; set; }
    }
}
