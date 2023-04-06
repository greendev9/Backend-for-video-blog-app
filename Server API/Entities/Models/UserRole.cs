using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UserRole : IMapInterface
    {
        public int LeftId { get; set; }
        public virtual User User { get; set; }
        public int RightId { get; set; }
        public virtual Role Role { get; set; }
    }
}
