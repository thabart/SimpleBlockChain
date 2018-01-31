﻿using System.Collections.Generic;

namespace Kmehr.EF.Models
{
    internal class HealthCarePartyType
    {
        public string Code { get; set; }
        public virtual ICollection<Translation> Translations { get; set; }
    }
}
