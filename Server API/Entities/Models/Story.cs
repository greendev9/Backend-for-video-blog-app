using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
    public class Story : IEntityBase
    {
        public int ID { get; set; }
        public StoryStatus StoryStatus { get; set; }
        public bool IsCommentsAlowed { get; set; }
        [StringLength(200)]
        public string Title { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        //[StringLength(1000)] //Alex : should be unlimited. dbo.Stories > Body data type changed (nvarchar(1000) -> nvarchar(MAX))
        public string Body { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public List<StoryTopic> StoryTopic { get; set; }
        public virtual ICollection<LikedUserStory> LikedUserStory { get; set; }
        public virtual ICollection<ViewedUserStory> ViewedUserStory { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }

        [StringLength(6)]
        public string Language { get; set; } 
        public bool? IsPoll { set; get; }

        public string PollData { set; get; }
        public bool? IsNew { get; set; }
    }
}
