using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{

     
    public class BlockedUser
    {   
        // who blocked
        public int LeftId { get; set; }
        public virtual User IUser { get; set; }
        // whom blocked
        public int RightId { get; set; }
        public virtual User OUser { get; set; }
        public string Comment { set; get; }
        public DateTime BlockDate { set; get; }
    }

}
