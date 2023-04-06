using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IDictionaryEntity:IEntityBase
    {
        int Index { get; set; }
        string Value { get; set; }
        bool IsSpecial { get; set; }
        int? FatherId { get; set; }
    }
}
