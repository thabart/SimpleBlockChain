using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kmehr.Core.Etk
{
    [XmlRoot("SearchCriteriaType")]
    public class SearchCriteriaType
    {
        [XmlElement("Identifier")]
        public List<EtkIdentifierType> Identifiers { get; set; }
    }
}
