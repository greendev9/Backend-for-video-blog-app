using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class UserComment : IMapInterface
    {
        public int LeftId { get; set; }
        public virtual User User { get; set; }
        public int RightId { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
