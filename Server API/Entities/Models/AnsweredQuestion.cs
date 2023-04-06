using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Enums;

namespace Entities
{
    public class AnsweredQuestion :IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public bool Yes { get; set; }
        public bool Maybe { get; set; }
        public bool NotSure { get; set; }
        public bool No { get; set; }
        public bool Noway { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime ApprovedDate { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(50)]
        public string SubCategory { get; set; }

        [StringLength(6)]
        public string Language { get; set; }

        public StoryStatus StoryStatus { get; set; }

        public Poll Poll { get; set; }
    }
}
