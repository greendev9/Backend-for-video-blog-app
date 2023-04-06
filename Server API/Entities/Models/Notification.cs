using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
    public class Notification : IEntityBase
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int CommentUserID { get; set; }
        public int? CommentId { get; set; }
        public int StoryId { get; set; }
        public User User { get; set; }
        [StringLength(255)]
        public string Message { get; set; }
        public int Type { get; set; }
        public byte IsRead { get; set; }
        public DateTime NotifDate { get; set; }
        public bool? ByAdmin { get; set; }
    }
}
