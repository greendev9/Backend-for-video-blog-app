using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
    public class Country : IDictionaryEntity
    {
        public int ID { get; set; }
        public int Index { get; set; }
        [StringLength(100)]
        public string Value { get; set; }
        [StringLength(100)]
        public int? FatherId { get; set; }
        public bool IsSpecial { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
