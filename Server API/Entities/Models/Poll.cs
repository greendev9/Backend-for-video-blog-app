using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
    public class Poll:IEntityBase
    {
        public int ID { get; set; }
        [StringLength(1000)]
        public string Question { get; set; }
        [StringLength(20)]
        public string FirstChoice { get; set; }
        [StringLength(20)]
        public string SecondChoice { get; set; }
        [StringLength(20)]
        public string ThirdChoice { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(50)]
        public string SubCategory { get; set; }

        [StringLength(6)]
        public string Language { get; set; }

        public virtual ICollection<AnsweredQuestion> AnsweredQuestion { get; set; }
        public StoryStatus StoryStatus { get; set; }

    }
}
