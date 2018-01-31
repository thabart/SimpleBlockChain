using System.Collections.Generic;

namespace Kmehr.EF.Models
{
    internal class Language
    {
        public string Id { get; set; }
        public virtual ICollection<Translation> Translations { get; set; }
    }
}
