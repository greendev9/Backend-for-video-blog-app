using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Language : IDictionaryEntity
    {
        public int ID { get; set; }
        public int Index { get; set; }
        [StringLength(100)]
        public string Value { get; set; }
        public bool IsSpecial { get; set; }
        public int? FatherId { get; set; } 

        public string Iso { set; get; }

    }
}
