using Domain;
using Entities;
using Entities.Enums;
using Microsoft.Extensions.Localization;
using Services.Posts;
using System;
using System.Linq;
using WebApi.Models.Messages;

namespace WebApi.Models
{
    public class NotificationModel
    {
        public int ID { get; set; }
        public int StoryOwnerUserID { get; set; }
        public int CommentUserID { get; set; }
        public int? CommentId { get; set; }
        public int StoryId { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public byte IsRead { get; set; }
        public string StoryTitle { get; set; }
        public string NotifDate { get; set; }
        public string NickName { get; set; }
        public string Color { get; internal set; }
        public string Gender { get; internal set; }

        public NotificationModel(Notification notificationData, Services.Customers.ICustomerService _customerService) { }
        public NotificationModel(Notification notif, Services.Customers.ICustomerService _customerService, IPostService _postService, IStringLocalizer _localizer, IStagingRepository<Comment> _commentsRepository)
        {
            var user = _customerService.GetUserByIdNotif(notif.UserID);
            var story = _postService.GetStoryByIdIncludeStoryTopic(notif.StoryId);

            ID = notif.ID;
            StoryOwnerUserID = notif.UserID;
            StoryId = notif.StoryId;
            CommentUserID = notif.CommentUserID;
            CommentId = notif.CommentId;
            StoryTitle = story.Title;

            //string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToUpper();
            var userComment = notif.CommentUserID != 0 ? _customerService.GetUserById(notif.CommentUserID) : null;
            string _nickName = userComment != null? userComment.NickName : string.Empty;
            //string _story_Title = story.Title;
            string _userComment = _commentsRepository.Table.Where(x => x.ID == CommentId).OrderByDescending(c => c.PublishDate).FirstOrDefault()?.Body;
            string ps = story.Category != null ? "p" : "s"; // Feed post if story.Category is NOT NULL and Community post if NULL
            string _communityName = story.Category != null ? _localizer[story.Category.ToLower().Trim()] : String.Empty;
            string _storyTopic = story.StoryTopic.Count() > 0 ?_localizer[story.StoryTopic.First().Topic.Value.ToLower().Trim().Replace(" ", "")] : "";
            switch (notif.Type)
            {
                case (int)NotificationType.STORY_APPROVED:
                    Message = _localizer[ps + "PostApproved"].Value;
                    break;
                case (int)NotificationType.STORY_PENDING:
                    Message = _localizer[ps + "PostPending"].Value;
                    break;
                case (int)NotificationType.STORY_REJECTED:
                    Message = _localizer[ps + "PostRejected"].Value;
                    break;
                case (int)NotificationType.USER_COMMENTED:
                    Message = _localizer[ps + "UserCommented"].Value;
                    break;
                case (int)NotificationType.USER_LIKED:
                    Message = _localizer[ps + "UserLiked"].Value;
                    break;
                default:
                    Message = String.Empty;
                    break;
            }
            Message = Message.Replace("story_Title", StoryTitle).Replace("nickName", _nickName).Replace("userComment", _userComment)
                .Replace("communityName", _communityName).Replace("storyTopic", _storyTopic);
            Type = ((Entities.Enums.NotificationType)notif.Type).ToString();
            IsRead = notif.IsRead;
            NotifDate = notif.NotifDate.ToString("dd.MM.yyyy");
            Color = userComment != null?userComment.ItemColor.ColorHex:user.ItemColor.ColorHex;
            NickName = user.NickName;
            Gender = userComment != null ? userComment.Gender.ToString() : user.Gender.ToString();
        }
    }

    public class NotificationModelHub
    {
        public NotificationModel notificationData { get; set; }
        public UserInfo userInfo { get; set; }
    }
}
