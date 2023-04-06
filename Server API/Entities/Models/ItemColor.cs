using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
    public class ItemColor : IDictionaryEntity
    {
        public int ID { get; set; }
        public int Index { get; set; }
        [StringLength(100)]
        public string Value { get; set; }
        [StringLength(100)]
        public string ColorHex { get; set; }
        public string FatherName { get; set; }
        public bool IsSpecial { get; set; }
        public int? FatherId { get; set; }
        public virtual IList<User> Users { get; set; }
    }
}
