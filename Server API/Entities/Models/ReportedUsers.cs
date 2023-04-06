using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class ReportedUsers
    {
        public int LeftId { get; set; }
        public virtual User IUser { get; set; }
        public int RightId { get; set; }
        public virtual User OUser { get; set; }
        public string Comment { set; get; }
        public DateTime ? ReportDate { set; get; }
    }

}
