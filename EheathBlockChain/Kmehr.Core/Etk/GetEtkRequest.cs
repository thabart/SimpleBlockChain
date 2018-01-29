using System.Xml.Serialization;

namespace Kmehr.Core.Etk
{
    [XmlRoot("GetEtkRequest")]
    public class GetEtkRequest
    {
        [XmlElement("SearchCriteria")]
        public SearchCriteriaType SearchCriteria { get; set; }
    }
}
