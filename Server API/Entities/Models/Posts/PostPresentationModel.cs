
using Entities;
using Entities.Enums;
using Entities.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Models.Posts
{
    public class PostPresentationModel
    {
        //public bool Mine { get; set; }
        public PostModel Story { get; set; }
        public IList<CommentExtended> Comments { get; set; }
        public int TotalComments { get; set; }
        public int TailedCount { get; set; }
        public bool LikedByMe { get; set; }
        public bool ViewedByMe { get; set; }
        public int UserId { get; set; }
        public bool TailedByMe { get; set; }
        public bool MyPost { set; get; }
        public string Category { set; get; }
        public string SubCategory { set; get; }
        public string UserNick { get; set; }
        public string UserColor { get; set; }
        public int UserAge { get; set; }
        public string UserGender { get; set; }
        public object Topic { get; set; }

        public PostPresentationModel() { }

        public PostPresentationModel(Story s, int ? myUserId, List<Story> TailedStories, User blockedUsers)
        {
            //Mine = s.UserID == userId || false;
            Comments = s.Comments?.Where(item => item.IsPendingApproval != true
                && item.IsDeniedApproval != true
                && !blockedUsers.HidenUser.Any(h =>  /*h.LeftId == myUserId && */h.RightId == item.UserId)
                && !blockedUsers.BlockedUser.Any(b => b.RightId == item.UserId && b.LeftId == myUserId)
                && !blockedUsers.BlockedUser.Any(b => b.LeftId == item.UserId && b.RightId == myUserId)
                && !blockedUsers.ReportedUsers.Any(h => h.RightId == item.UserId && h.LeftId == myUserId)
                && !blockedUsers.ReportedUsers.Any(h => h.LeftId == item.UserId && h.RightId == myUserId)
                )
                .OrderByDescending(c => c.ID).Take(2).Select(k =>
                new CommentExtended
                {
                    Comment = new CommentModel(k),
                    User = k.User != null ? new UserInCommentModel(k.User) : null,
                    ItemColor = k.User.ItemColor != null ? new ColorModel(k.User.ItemColor) : null
                }).ToList();
                
            TotalComments = s.Comments != null ? s.Comments.Count() : 0;
            Story = new PostModel(s);
            Story.ViewedUserStory = null;
            ViewedByMe = s.ViewedUserStory != null ? s.ViewedUserStory.Any(z => z.LeftId == myUserId) : false;
            UserId = s.UserID;
            TailedByMe = TailedStories.Any(t => t.ID == s.ID);
            MyPost = s.UserID == myUserId;
            Category = s.Category;
            SubCategory = s.SubCategory;
        }
    }

    public class PostModel
    {
        public int ID { get; set; }
        public StoryStatus StoryStatus { get; set; }
        public bool IsCommentsAlowed { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public int UserID { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public List<StoryTopic> StoryTopic { get; set; }
        public virtual ICollection<LikedUserStory> LikedUserStory { get; set; }
        public virtual ICollection<ViewedUserStory> ViewedUserStory { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Language { get; set; }
        public bool? IsPoll { set; get; }
        public string PollData { set; get; }
        public bool? IsNew { get; set; }
        //public User _User { get; set; }
        public UserPostModel User { get; set; }
        public PostModel() { }
        public PostModel(Story s)
        {
            ID = s.ID;
            StoryStatus = s.StoryStatus;
            IsCommentsAlowed = s.IsCommentsAlowed;
            Title = s.Title;
            Description = s.Description;
            Body = s.Body;
            UserID = s.UserID;
            PublishDate = s.PublishDate;
            ApprovedDate = s.ApprovedDate;
            StoryTopic = (from x in s.StoryTopic select new StoryTopic { LeftId = x.LeftId, RightId = x.RightId, 
            Topic = new Topic { ID = x.Topic.ID, FatherId = x.Topic.FatherId, Value = x.Topic.Value } }).ToList();
            LikedUserStory = s.LikedUserStory;
            ViewedUserStory = s.ViewedUserStory;
            Views = s.Views;
            Likes = s.Likes;
            Category = s.Category;
            SubCategory = s.SubCategory;
            Language = s.Language;
            IsPoll = s.IsPoll;
            PollData = s.PollData;
            IsNew = s.IsNew;
            // Check (GET)/api/v1/stories
            User = s.User != null ? new UserPostModel(s.User) : null;
            
        }
    }

    public class StoryTopicsModel
    {
        public int LeftId { get; set; }
        public int RightId { get; set; }
        public string Topic { get; set; }
    }

    public class StoryModelCron
    {
        public PostPresentationModel storyData { get; set; }
        public IList<User> receivers { get; set; }
        public IList<Notification> notificationData { get; set; }
    }
}
