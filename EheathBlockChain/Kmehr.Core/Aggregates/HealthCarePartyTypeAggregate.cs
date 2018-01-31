using System.Collections.Generic;

namespace Kmehr.Core.Models
{
    public class HealthCarePartyTypeAggregateDescription
    {
        public string Language { get; set; }
        public string Value { get; set; }
    }

    public class HealthCarePartyTypeAggregate
    {
        public string Code { get; set; }
        public IEnumerable<HealthCarePartyTypeAggregateDescription> Descriptions { get; set; }
    }
}
