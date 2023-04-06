
using Domain.Models.Polls;
using Domain.Models.Posts;
using Entities;
using Entities.Enums;
using Entities.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Posts
{

    public interface IPostService
    {
        IEnumerable<PostPresentationModel> SearchStories(
           string Catalog = null,
            string SubCatalog = null,
            int? UserId = null,
            string SearchTerm = null,
            StoryStatus status = StoryStatus.Approved,
            bool SkipFilters = false,
            bool OnlyMine = false,
            bool OnlyFromFollowingUsers = false,
            string Title = null,
            int skip = 0,
            int pageSize = int.MaxValue,
            List<int> topics = null, bool OnlyPolls = false, DateFilterType dateFilter = DateFilterType.NONE,
            bool skipCatalogs = false, bool includeUser = true);


        PollModel GetPoll(int storyId);

        void InsertOrdUpdatePoll(PollModel model, int userId);
        List<Notification> GenerateNotifications(Story story, bool byAdmin);
        Notification AddStoryApprovedNotification(Story story, bool byAdmin);
        List<Notification> InsertStory(Story story);
        void AddStoryForTailedCounting(Story story);
        void DeleteStoryForTailedCounting(int sId);
        void ClearBlaBlogNotif(int id);
        List<Notification> UpdateStory(Story story);
        Notification InsertComment(Comment comment, bool addNotification);
        List<BlockedUser> GetListBlockedByAdmin();
        IList<PollVariantOption> GetPollVariantOptions(int storyId, int userId);
        bool Vote(int storyId, string option, int iD);
        Story GetStoryById(int iD);
        Story GetStoryByIdIncludeStoryTopic(int iD);
        List<Notification> DenyStory(int userId, int storyId);
        Notification LikeStory(int userId, int storyId);
        bool RemoveComment(int userId, int id);
        void ViewStory(int userId, int storyId);
        int UpdateStatusPendingStories();
        void UpdateComment(Comment model);
        void UpdateCommentStatus(Comment model, bool byAdmin);
        void DeleteStory(int id);
        void ToggleComments(int id);
        //int GetStatistics(int userId, StoryStatus notApproved);
        UserTabStatModel GetUserTabStat(int userId);
        IEnumerable<CommentExtended> LoadComments(int postId, int skip, int limit);
        IList<Notification> GetNewNotifications(int userId, bool isAllNotifs);
        string GetUserColor(int id);
        string GetUserGender(int id);
        PostPresentationModel GetStoryModel(int id, int myUserId, User blockedUsers);
        void ClearCommentNotif(int id, int lastCommentNotificationID);  // Not used for now
        void ClickBell(int userId);
        Notification SetNotifStatus(int notification_id, byte isRead);
        int NewImTailingCount(int Id);
        bool IsSpam(int userId);
        List<StoryModelCron> CheckPendingStatus();
        List<StoryModelCron> GetApprovedStories();
        StoryModelCron SendStoryApproved(int sId);
        User GetBlockedHiddenReported(int? uId, BlockedRule blockedRule);
        bool IsUserBlockedMessages(int userId, int iD, User blockedUsers, out string notAllowedResponse);
        List<int> GetApprovedByAdmin();
        void ResetFlagChangedByAdmin(NotificationType notificationType);
        List<User> GetNewStoryAlertReceivers(Story s, User blockedUsers);
        int ClearBrokenLinks();
        List<string> GetBadWordsCach();
    }
}
