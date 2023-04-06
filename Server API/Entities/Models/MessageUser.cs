using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class MessageUser:IEntityBase
    {
        public int ID { get; set; }
        public int LeftId { get; set; }
        public int RightId { get; set; }
        [StringLength(1000)]
        public string Message { get; set; }
        public DateTime MessageDate { get; set; }
        public bool IsRead { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? IsClearedRight { get; set; }
        public bool? IsClearedLeft { get; set; }
        [NotMapped]
        public User OUser {set;get;}
    }
}
