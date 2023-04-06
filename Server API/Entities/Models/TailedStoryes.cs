using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class TailedStoryes
    {
        public int uId { get; set; }
        public virtual User IUser { get; set; }
        public int sId { get; set; }
    }
}
