using System.Collections.Generic;
using Domain;
using Entities;
using Entities.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Posts;
using Entities.Extended;
using System;
using Newtonsoft.Json;
using Domain.Models.Polls;
using System.Text.RegularExpressions;
using Domain.Helpers;
using DAL;
using MoreLinq;
using Domain.Models.Notifications;
using Microsoft.Extensions.Configuration;
using Services.Customers;
using Microsoft.Extensions.Caching.Memory;
//using Domain.Models.Customers;

namespace Services.Posts
{


  public class PostService : IPostService
  {
    //private const int spamTimeGap = 300; //5h.
    private readonly IStagingRepository<Story> _repository;
    private readonly IStagingRepository<TailedUsers> _tailedUsersRepository;
    private readonly IStagingRepository<TailedStoryes> _tailedStoryesRepository;
    private readonly IStagingRepository<Word> _wordRepository;
    private readonly IStagingRepository<User> _usersRepository;
    private readonly IStagingRepository<BlockedUser> _blockedRepository;
    private readonly IStagingRepository<HidenUser> _hiddenRepository;
    private readonly IStagingRepository<ReportedUsers> _reportRepository;
    private readonly IStagingRepository<Language> _languagesRepository;
    private readonly IStagingRepository<Vote> _votesRepository;
    private readonly IStagingRepository<Comment> _commentsRepository;
    private readonly IStagingRepository<ItemColor> _colorRepository;
    private readonly IStagingRepository<LikedUserStory> _likesRepository;
    private readonly IStagingRepository<ViewedUserStory> _viewsRepository;
    private readonly IStagingRepository<Topic> _topicRepository;
    private readonly IStagingRepository<Notification> _notificationsRepository;
    private readonly IStagingRepository<UserRole> _roleRepository;
    private readonly ICustomerService _customerService;
    private readonly IConfiguration _configuration;
    private IMemoryCache cacheBadWords;

    public PostService(IStagingRepository<Story> repo,
        IStagingRepository<UserRole> roleRepository,
        IStagingRepository<ItemColor> colorRepository,
        IStagingRepository<TailedUsers> tailedUsers,
        IStagingRepository<TailedStoryes> tailedStoryes,
        IStagingRepository<Word> wordRepository,
        IStagingRepository<User> usersRepository,
        IStagingRepository<BlockedUser> blockedRepo,
        IStagingRepository<HidenUser> hiddenRepository,
        IStagingRepository<ReportedUsers> reportRepository,
        IStagingRepository<Vote> voteRepository,
        IStagingRepository<Language> langRepository,
        IStagingRepository<LikedUserStory> likesRepository,
        IStagingRepository<Comment> commentsRepository,
        IStagingRepository<Topic> topicRepositry,
        IStagingRepository<ViewedUserStory> viewsRepository,
        IStagingRepository<Notification> notificationsRepository,
        ICustomerService customerService,
        IConfiguration config,
        IMemoryCache memoryCache)
    {
      _repository = repo;
      _roleRepository = roleRepository;
      _tailedUsersRepository = tailedUsers;
      _tailedStoryesRepository = tailedStoryes;
      _wordRepository = wordRepository;
      _usersRepository = usersRepository;
      _blockedRepository = blockedRepo;
      _hiddenRepository = hiddenRepository;
      _reportRepository = reportRepository;
      _votesRepository = voteRepository;
      _colorRepository = colorRepository;
      _likesRepository = likesRepository;
      _viewsRepository = viewsRepository;
      _commentsRepository = commentsRepository;
      _topicRepository = topicRepositry;
      _languagesRepository = langRepository;
      _notificationsRepository = notificationsRepository;
      _customerService = customerService;
      _configuration = config;
      cacheBadWords = memoryCache;
    }

    public IEnumerable<PostPresentationModel> SearchStories(
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
        List<int> topics = null,
        bool OnlyPolls = false,
        DateFilterType dateFilter = DateFilterType.NONE,
        bool skipCatalogs = false,
        bool includeUser = true)
    {
      var user = _usersRepository.GetById(UserId);
      var langu = _languagesRepository.GetById(user.LanguageId);
      var logedUserAge = Domain.Helpers.DateHelper.GetAge(user.YearOfBirth);
      var ageGroup = Domain.Helpers.DateHelper.GetAgeGroup(logedUserAge);
      var TailedIds = _tailedUsersRepository.Table.Where(p => p.LeftId == UserId && p.RightId != UserId).Select(p => p.RightId);
      var LikedByMe = _likesRepository.Table.Where(p => p.LeftId == UserId).Select(p => p.RightId);
      var TailedStories = _repository.TableNoTracking.Where(p => TailedIds.Any(t => p.UserID == t)).ToList();
      List<int> usersByAge =
      _usersRepository.TableNoTracking.Where(p =>
                     Domain.Helpers.DateHelper.GetAge(p.YearOfBirth) >= ageGroup.Key &&
                     Domain.Helpers.DateHelper.GetAge(p.YearOfBirth) <= ageGroup.Value).Select(p => p.ID).ToList();


      var posts = (from post in _repository.Table
                   where
                           post.User.Deactivated != true && SkipFilters == false
                           && usersByAge.Contains(post.UserID) && SkipFilters == false
                           && post.User.CountryId == (user.CountryId) && SkipFilters == false
                           && post.User.LanguageId == (user.LanguageId) && SkipFilters == false
                           && (post.Language == (user.LanguageId == 1 ? "he" : "en")
                           || (langu.Iso.ToUpper() == post.Language.ToUpper() || post.UserID == UserId))
                           && SkipFilters == false
                   select post).AsQueryable();

      if (includeUser)
        posts = posts.Include(p => p.User).ThenInclude(p => p.ItemColor)
            .Include(p => p.StoryTopic).ThenInclude(p => p.Topic).AsQueryable();

      if (OnlyPolls)
        posts = from post in posts where post.IsPoll == true select post;
      else if (!SkipFilters)
        posts = from post in posts where post.IsPoll != true select post;

      if (SkipFilters)
        posts = (from post in _repository.Table select post).Include(p => p.User).ThenInclude(p => p.ItemColor).
            Include(p => p.StoryTopic).ThenInclude(p => p.Topic).AsQueryable();

      if (OnlyFromFollowingUsers)
        posts = from post in posts where TailedIds.Contains(post.UserID) select post;


      if (!SkipFilters && topics != null && topics.Count > 0)
        posts = from post in posts where topics.Any(t => post.StoryTopic.Any(s => s.RightId == t)) select post;


      if (status != StoryStatus.None)
        posts = from post in posts where post.StoryStatus == status select post;

      var blockedUsersByAdmin = GetListBlockedByAdmin();
      posts = from post in posts where !blockedUsersByAdmin.Any(h => h.RightId == post.UserID) select post;

      var blockedUsers = GetBlockedHiddenReported(UserId, BlockedRule.ForPosts);

      if (!SkipFilters)
        posts = posts.Where(post =>
            //Hidden user. When UserA hides UserB: UserA can't see UserB in feed but UserB can see UserA
            !blockedUsers.HidenUser.Any(h => h.RightId == post.UserID)
            //Blocked user. When UserA blocked UserB: UserA and UserB can't see each other (reqs changed)
            && (!blockedUsers.BlockedUser.Any(b => b.RightId == post.UserID && b.LeftId == UserId)
            && !blockedUsers.BlockedUser.Any(b => b.LeftId == post.UserID && b.RightId == UserId))
            //Reported user. When UserA reported UserB: UserA and UserB can't see each other
            && !blockedUsers.ReportedUsers.Any(r => r.RightId == post.UserID && r.LeftId == UserId)
            && !blockedUsers.ReportedUsers.Any(r => r.LeftId == post.UserID && r.RightId == UserId)
        );

      if (OnlyMine)
        posts = from post in posts where post.UserID == UserId select post;


      if (!string.IsNullOrEmpty(Title) && !SkipFilters)
        posts = from post in posts where (post.Title != null ? post.Title.ToLower() : " ").Contains(Title.ToLower()) || post.Body.ToLower().Contains(Title.ToLower()) select post; // added search in Body

      if (!skipCatalogs)
      {
        if (!string.IsNullOrEmpty(Catalog) && !SkipFilters)
          posts = from post in posts where post.Category.ToLower() == Catalog.ToLower() select post;
        if (!string.IsNullOrEmpty(SubCatalog) && !SkipFilters)
          posts = from post in posts where post.SubCategory.ToLower() == SubCatalog.ToLower() select post;
      }
      else if (!SkipFilters)
      {
        posts = from post in posts where post.Category == null select post;
      }


      if (dateFilter != DateFilterType.NONE)
      {
        DateTime MoreThenDates = new DateTime();
        switch (dateFilter)
        {
          case DateFilterType.TODAY:
            //MoreThenDates = DateTime.Now.Date;
            MoreThenDates = DateTime.Now.AddDays(-1);
            break;
          case DateFilterType.WEEK:
            //MoreThenDates = DateTime.Now.StartOfWeek(DayOfWeek.Sunday).Date;
            MoreThenDates = DateTime.Now.AddDays(-7);
            break;
          case DateFilterType.MONTH:
            //MoreThenDates = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
            MoreThenDates = DateTime.Now.AddDays(-30);
            break;
          case DateFilterType.YEAR:
            //MoreThenDates = new DateTime(DateTime.Now.Year, 1, 1).Date;
            MoreThenDates = DateTime.Now.AddDays(-365);
            break;
          default:
            break;
        }
        posts = posts.Where(p => p.ApprovedDate > MoreThenDates);
      }

      posts = posts.Include(c => c.Comments).ThenInclude(u => u.User).ThenInclude(i => i.ItemColor)
          .Include(st => st.StoryTopic).ThenInclude(t => t.Topic).Include(p => p.ViewedUserStory);


      var result = posts.OrderByDescending(post => post.PublishDate)
      .Skip(skip).Take(pageSize).Select(p => new PostPresentationModel(p, UserId, TailedStories, blockedUsers)).ToList();

      result.ForEach(x =>
      {
        x.TailedCount = TailedIds.Count();
        x.LikedByMe = LikedByMe.Contains(x.Story.ID);
        x.UserAge = DateTime.Now.Year - _usersRepository.GetById(x.UserId).YearOfBirth;
        x.UserNick = _usersRepository.GetById(x.UserId).NickName;
        x.UserColor = _colorRepository.GetById(_usersRepository.GetById(x.UserId).ItemColorID).ColorHex;
        x.UserGender = _usersRepository.GetById(x.UserId).Gender.ToString();
        x.Topic = null;
        if (x.Story.StoryTopic.Count > 0)
          x.Topic = _topicRepository.GetById(x.Story.StoryTopic.FirstOrDefault().RightId).Value;
      });

      return result;
    }

    //Users is not able to send messages each other thru inbox (blocked, hidden, reported)
    public bool IsUserBlockedMessages(int fromUserId, int toUserId, User blockedUsers, out string message)
    {
      message = String.Empty;
      bool notAllowed = false;

      if (blockedUsers.HidenUser.Any(h => h.LeftId == toUserId && h.RightId == fromUserId))
      {
        message = "HidenUser";
        notAllowed = true;
      }
      else if (blockedUsers.HidenUser.Any(h => h.RightId == toUserId && h.LeftId == fromUserId))
      {
        message = "HidenUser";
        notAllowed = true;
      }
      else if (blockedUsers.BlockedUser.Any(b => b.LeftId == toUserId && b.RightId == fromUserId))
      {
        message = "YouBlockedByUser";
        notAllowed = true;
      }
      else if (blockedUsers.BlockedUser.Any(b => b.RightId == toUserId && b.LeftId == fromUserId))
      {
        message = "YouBlockedThisUser";
        notAllowed = true;
      }
      else if (blockedUsers.ReportedUsers.Any(r => r.RightId == toUserId && r.LeftId == fromUserId))
      {
        message = "YouReportedThisUser";
        notAllowed = true;
      }
      else if (blockedUsers.ReportedUsers.Any(r => r.LeftId == toUserId && r.RightId == fromUserId))
      {
        message = "YouReportedByUser";
        notAllowed = true;
      }

      return notAllowed;
    }

    public PostPresentationModel GetStoryModel(int storyId, int myUserId, User blockedUsers)
    {
      var post = _repository.Table.Where(s => s.ID == storyId).Include(c => c.Comments)
          .ThenInclude(c => c.User).ThenInclude(p => p.ItemColor)
          .Include(p => p.StoryTopic).ThenInclude(p => p.Topic).Include(u => u.User).ThenInclude(p => p.ItemColor).FirstOrDefault();
      var TailedIds = _tailedUsersRepository.Table.Where(p => p.LeftId == post.UserID).Select(p => p.RightId);
      var TailedStories = _repository.TableNoTracking.Where(p => TailedIds.Any(t => p.UserID == t)).ToList();
      var LikedByMe = _likesRepository.Table.Where(p => p.LeftId == myUserId).Select(p => p.RightId);

      var result = new PostPresentationModel(post, myUserId, TailedStories, blockedUsers);
      result.TailedCount = TailedIds.Count();
      result.LikedByMe = LikedByMe.Contains(result.Story.ID);
      result.UserAge = DateTime.Now.Year - _usersRepository.GetById(result.UserId).YearOfBirth;
      result.UserNick = _usersRepository.GetById(result.UserId).NickName;
      result.UserColor = _colorRepository.GetById(_usersRepository.GetById(result.UserId).ItemColorID).ColorHex;
      result.UserGender = _usersRepository.GetById(result.UserId).Gender.ToString();
      result.Topic = null;
      if (result.Story.StoryTopic.Count > 0)
        result.Topic = _topicRepository.GetById(result.Story.StoryTopic.FirstOrDefault().RightId).Value;

      return result;
    }

    public List<BlockedUser> GetListBlockedByAdmin()
    {
      //var bl = _blockedRepository.Table.Join(_roleRepository.Table, p => p.LeftId, q => q.Role.ID, (p, q) => new { p, q });
      //bl = bl.Where(r => r.q.Role.Value == "Admin");

      var admins = _roleRepository.Table.Where(t => t.Role.Value == "Admin");
      return _blockedRepository.TableNoTracking.Where(b => admins.Any(a => a.LeftId == b.LeftId)).ToList();
    }

    //public List<Report> GetListReported(string nickName)
    //{
    //    return _reportRepository.TableNoTracking.Where(b => b.Reported == nickName).ToList();
    //}

    /// <summary>
    /// GetPollVariantOptions
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public IList<PollVariantOption> GetPollVariantOptions(int storyId, int userId)
    {
      var result = new List<PollVariantOption>();
      var defaultoptions = JsonConvert.DeserializeObject<PollModel>(_repository.GetById(storyId).PollData);


      var votes = (from vote in _votesRepository.Table
                   where vote.StoryId == storyId
                   select vote);


      var looked = votes.DistinctBy(x => x.OptionSelected).ToLookup(x => x.OptionSelected.Replace(" ", ""));

      foreach (var item in looked)
      {
        result.Add(new PollVariantOption
        {
          VariantName = item.Key,
          Percentage = (int)(((double)item.Count() / votes.Count()) * 100),
          Voted = item.Any(v => v.UserId == userId)
        });
      }

      if (!string.IsNullOrEmpty(defaultoptions.FirstChoice) && !result.Any(x => x.VariantName == defaultoptions.FirstChoice))
        result.Add(new PollVariantOption { VariantName = defaultoptions.FirstChoice, Percentage = 0 });

      if (!string.IsNullOrEmpty(defaultoptions.SecondChoice) && !result.Any(x => x.VariantName == defaultoptions.SecondChoice))
        result.Add(new PollVariantOption { VariantName = defaultoptions.SecondChoice, Percentage = 0 });

      if (!string.IsNullOrEmpty(defaultoptions.ThirdChoice) && !result.Any(x => x.VariantName == defaultoptions.ThirdChoice))
        result.Add(new PollVariantOption { VariantName = defaultoptions.ThirdChoice, Percentage = 0 });

      if (defaultoptions.Yes && !result.Any(x => x.VariantName == "Yes"))
        result.Add(new PollVariantOption { VariantName = "Yes", Percentage = 0 });

      if (defaultoptions.Maybe && !result.Any(x => x.VariantName == "Maybe"))
        result.Add(new PollVariantOption { VariantName = "Maybe", Percentage = 0 });

      if (defaultoptions.NotSure && !result.Any(x => x.VariantName.Replace(" ", "") == "NotSure"))
        result.Add(new PollVariantOption { VariantName = "Not Sure", Percentage = 0 });

      if (defaultoptions.No && !result.Any(x => x.VariantName == "No"))
        result.Add(new PollVariantOption { VariantName = "No", Percentage = 0 });

      if (defaultoptions.Noway && !result.Any(x => x.VariantName.Replace(" ", "") == "NoWay"))
        result.Add(new PollVariantOption { VariantName = "No Way", Percentage = 0 });

      return result.DistinctBy(x => x.VariantName).OrderBy(r => r.VariantName).ToList();
    }

    /// <summary>
    /// Vote 
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="option"></param>
    /// <param name="iD"></param>
    /// <returns></returns>
    public bool Vote(int storyId, string option, int iD)
    {
      var o = Regex.Split(option, @"(?<!^)(?=[A-Z])");

      if (_votesRepository.Table.Any(v => v.StoryId == storyId && v.UserId == iD))
      {
        var prevSelected = _votesRepository.Table.FirstOrDefault(v => v.StoryId == storyId && v.UserId == iD);
        _votesRepository.Delete(prevSelected);
      }

      _votesRepository.Insert(new Domain.Models.Polls.Vote
      {
        UserId = iD,
        StoryId = storyId,
        OptionSelected = string.Join(' ', o)
      });
      return true;

    }

    public List<Notification> GenerateNotifications(Story story, bool byAdmin)
    {
      List<Notification> notifs = new List<Notification>();
      //string msg = story.StoryTopic == null ? "post" : "story"; // Feed post (STORY) if StoryTopic is NOT NULL and Community POST if StoryTopic is NULL
      Notification notif = new Notification { UserID = story.UserID, CommentUserID = 0, StoryId = story.ID, IsRead = 0, NotifDate = DateTime.Now };
      switch (story.StoryStatus)
      {
        case StoryStatus.Approved:
          notif.ID = 0;
          notif.Type = (int)NotificationType.STORY_PENDING;
          notif.Message = "NotifStorytPendingApproval";
          notif.ByAdmin = byAdmin;
          _notificationsRepository.Insert(notif);
          notifs.Add(notif);
          notif = new Notification { UserID = story.UserID, CommentUserID = 0, StoryId = story.ID, IsRead = 0, NotifDate = DateTime.Now };
          notif.Type = (int)NotificationType.STORY_APPROVED;
          notif.Message = "NotifStorytApproved";
          notif.ByAdmin = byAdmin;
          _notificationsRepository.Insert(notif);
          notifs.Add(notif);
          AddStoryForTailedCounting(story);
          break;
        case StoryStatus.PendingApproval:
          notif.ID = 0;
          notif.Type = (int)NotificationType.STORY_PENDING;
          notif.Message = "NotifStorytPendingApproval";
          notif.ByAdmin = byAdmin;
          _notificationsRepository.Insert(notif);
          notifs.Add(notif);
          DeleteStoryForTailedCounting(story.ID);
          break;
        case StoryStatus.NotApproved:
          notif.ID = 0;
          notif.Type = (int)NotificationType.STORY_REJECTED;
          notif.Message = "NotifStoryDenied";
          notif.ByAdmin = byAdmin;
          _notificationsRepository.Insert(notif);
          notifs.Add(notif);
          DeleteStoryForTailedCounting(story.ID);
          break;
        default:
          break;
      }
      return notifs;
    }

    public Notification AddStoryApprovedNotification(Story story, bool byAdmin)
    {
      Notification notif = new Notification { UserID = story.UserID, CommentUserID = 0, StoryId = story.ID, IsRead = 0, NotifDate = DateTime.Now, ByAdmin = byAdmin };
      notif.Type = (int)NotificationType.STORY_APPROVED;
      notif.Message = "NotifStorytApproved";
      _notificationsRepository.Insert(notif);
      AddStoryForTailedCounting(story);
      return notif;
    }

    public void AddStoryForTailedCounting(Story story)
    {
      var tailingUsers = _tailedUsersRepository.Table.Where(t => t.RightId == story.UserID).Select(i => i.LeftId).ToList();
      tailingUsers.ForEach(u =>
      {
        bool isExist = _tailedStoryesRepository.Table.Any(t => t.uId == u && t.sId == story.ID);
        if (!isExist)
          _tailedStoryesRepository.Insert(new TailedStoryes { uId = u, sId = story.ID });
      });
    }

    public void DeleteStoryForTailedCounting(int sId)
    {
      var tailedStoryes = _tailedStoryesRepository.Table.Where(s => s.sId == sId).ToList();
      tailedStoryes.ForEach(t =>
      {
        _tailedStoryesRepository.Delete(t);
      });
    }

    public void ClearBlaBlogNotif(int uId)
    {
      var tailedStoryes = _tailedStoryesRepository.Table.Where(s => s.uId == uId).ToList();
      tailedStoryes.ForEach(t =>
      {
        _tailedStoryesRepository.Delete(t);
      });
    }

    public List<Notification> InsertStory(Story story)
    {
      _repository.Insert(story);

      //Return notifications
      return GenerateNotifications(story, false);
    }

    public List<Notification> UpdateStory(Story story)
    {
      _repository.Update(story);
      //Return notifications
      return GenerateNotifications(story, false);
    }

    public Story GetStoryById(int iD)
    {
      return _repository.Table.Where(s => s.ID == iD).Include(c => c.Comments).FirstOrDefault();
    }

    public Story GetStoryByIdIncludeStoryTopic(int iD)
    {
      return _repository.Table.Where(s => s.ID == iD).Include(c => c.StoryTopic).ThenInclude(p => p.Topic).FirstOrDefault();
    }

    public List<Notification> DenyStory(int userId, int storyId)
    {
      var story = _repository.GetById(storyId);
      story.StoryStatus = StoryStatus.NotApproved;
      _repository.Update(story);
      //Return notifications
      return GenerateNotifications(story, false);
    }

    public Notification LikeStory(int userId, int storyId)
    {
      LikedUserStory lus = new LikedUserStory { LeftId = userId, RightId = storyId };
      var story = _repository.Table.Include(x => x.LikedUserStory).FirstOrDefault(s => s.ID == storyId);

      if (!story.LikedUserStory.Any(x => x.LeftId == userId))
      {
        story.Likes++;
        story.IsNew = true;
        _repository.Update(story);
        _likesRepository.Insert(lus);

        string nickName = _usersRepository.GetById(userId).NickName;
        Notification notification = new Notification
        {
          Message = "NotifLikedStory",
          Type = (int)NotificationType.USER_LIKED,
          UserID = story.UserID, // Story Owner ID
          CommentUserID = userId,
          StoryId = story.ID,
          IsRead = 0,
          NotifDate = DateTime.Now
        };
        _notificationsRepository.Insert(notification);
        return notification;
      }
      return null;
    }

    public void ViewStory(int userId, int storyId)
    {
      ViewedUserStory lus = new ViewedUserStory { LeftId = userId, RightId = storyId };
      var story = _repository.Table.Include(x => x.ViewedUserStory).FirstOrDefault(s => s.ID == storyId);

      if (!story.ViewedUserStory.Any(x => x.LeftId == userId))
      {
        story.Views++;
        _repository.Update(story);
        _viewsRepository.Insert(lus);
      }
    }

    public void DeleteStory(int id)
    {
      var DB = _repository.GetContext() as DataContext;
      DB.Database.ExecuteSqlCommand($"Delete from [dbo].[Comments] Where StoryId={id}");
      DB.Database.ExecuteSqlCommand($"Delete from [dbo].[LikedUserStories] Where RightId={id}");
      DB.Database.ExecuteSqlCommand($"Delete from [dbo].[StoryTopics] Where RightId={id}");
      DB.Database.ExecuteSqlCommand($"Delete from [dbo].[ViewedUserStories] Where RightId={id}");
      DB.Database.ExecuteSqlCommand($"Delete from [dbo].[Notifications] Where StoryId={id}");
      DB.Database.ExecuteSqlCommand($"Delete from [dbo].[Votes] Where StoryId={id}");

      DB.Database.ExecuteSqlCommand($"Delete from [dbo].[Storyes] Where ID={id}");

      //var story = _repository.GetById(id);
      //_repository.Delete(story);
    }

    public void ToggleComments(int id)
    {
      var story = _repository.GetById(id);
      story.IsCommentsAlowed = story.IsCommentsAlowed ? false : true;
      _repository.Update(story);
    }

    //public int GetStatistics(int userId, StoryStatus notApproved)
    //{
    //    var q = from s in _repository.Table where s.UserID == userId && s.StoryStatus == notApproved select s;
    //    return q.Count();
    //}

    public UserTabStatModel GetUserTabStat(int userId)
    {
      var q = _repository.Table.Where(s => s.UserID == userId).ToList();// from s in _repository.Table where s.UserID == userId select s.StoryStatus;
      var qDeniedPublished = q.Where(d => d.StoryStatus == StoryStatus.NotApproved);
      var qTotalApproval = q.Where(d => d.StoryStatus == StoryStatus.Approved);
      var qTotalDrafts = q.Where(d => d.StoryStatus == StoryStatus.Draft);
      var qTotalPending = q.Where(d => d.StoryStatus == StoryStatus.PendingApproval);
      UserTabStatModel stat = new UserTabStatModel
      {
        DeniedPublished = qDeniedPublished.Count(),
        TotalApproval = qTotalApproval.Count(),
        TotalDrafts = qTotalDrafts.Count(),
        TotalPending = qTotalPending.Count()
      };
      return stat;
    }

    public Notification InsertComment(Comment comment, bool addNotification)
    {
      //insert comment
      _commentsRepository.Insert(comment);
      var story = _repository.Table.FirstOrDefault(x => x.ID == comment.StoryId);
      story.IsNew = true;
      _repository.Update(story);


      if (addNotification) //insert comment notification
      {
        Notification notification = new Notification
        {
          Message = "NotifCommentedStory",
          Type = (int)NotificationType.USER_COMMENTED,
          UserID = story.UserID, // Story Owner ID
          CommentUserID = comment.UserId,
          CommentId = comment.ID,
          StoryId = story.ID,
          IsRead = 0,
          NotifDate = DateTime.Now
        };
        _notificationsRepository.Insert(notification);
        return notification;
      }
      return null;
    }

    public bool RemoveComment(int userId, int id)
    {
      bool result = true;
      var comment = _commentsRepository.GetById(id);
      if (comment == null)
        return false;
      var userPostOwner = _repository.GetById(comment.StoryId).UserID;
      if (comment.UserId == userId || userPostOwner == userId)
      {
        _commentsRepository.Delete(comment);
        var notif = _notificationsRepository.Table.Where(c => c.CommentId == id).FirstOrDefault();
        if (notif != null)
          _notificationsRepository.Delete(notif);
      }
      else
        result = false;
      return result;
    }

    public IEnumerable<CommentExtended> LoadComments(int postId, int skip, int limit)
    {
      var blockedUsersByAdmin = GetListBlockedByAdmin();
      var result = _commentsRepository.Table.Where(x => x.StoryId == postId && !blockedUsersByAdmin.Any(h => h.RightId == x.UserId) && x.IsPendingApproval != true
          && x.IsDeniedApproval != true).OrderByDescending(c => c.PublishDate)
          .Include(u => u.User).ThenInclude(u => u.ItemColor)
          .Select(x => x).Skip(skip).Take(limit).ToList();

      result.ForEach(c => { c.User.Comments = null; });

      return result.Select(x => new CommentExtended
      {
        Comment = new CommentModel(x),
        User = x.User != null ? new UserInCommentModel(x.User) : null,
        ItemColor = x.User != null ? new ColorModel(x.User.ItemColor) : null
      }).ToList();
    }

    public void UpdateComment(Comment model)
    {
      var comment = _commentsRepository.GetById(model.ID);
      if (comment != null)
        comment.Body = model.Body;
      {
        _commentsRepository.Update(comment);
      }
    }

    public void UpdateCommentStatus(Comment model, bool byAdmin)
    {
      var comment = _commentsRepository.GetById(model.ID);
      if (comment != null)
      {
        comment.IsPendingApproval = model.IsPendingApproval;
        comment.IsDeniedApproval = model.IsDeniedApproval;
      }
      _commentsRepository.Update(comment);
      var story = _repository.Table.FirstOrDefault(x => x.ID == comment.StoryId);
      story.IsNew = true;
      _repository.Update(story);


      if (model.IsPendingApproval == false && model.IsDeniedApproval == false) //insert comment notification
      {
        Notification notification = new Notification
        {
          Message = "NotifCommentedStory",
          Type = (int)NotificationType.USER_COMMENTED,
          UserID = story.UserID, // Story Owner ID
          CommentUserID = comment.UserId,
          CommentId = comment.ID,
          StoryId = story.ID,
          IsRead = 0,
          NotifDate = DateTime.Now,
          ByAdmin = byAdmin
        };
        _notificationsRepository.Insert(notification);
      }
    }

    public IList<Notification> GetNewNotifications(int userId, bool isAllNotifs)
    {
      List<Notification> source = new List<Notification>();
      if (isAllNotifs)
        source = _notificationsRepository.Table.Where(x => x.UserID == userId && x.CommentUserID != userId).OrderByDescending(x => x.ID).ToList();
      else // unread notifs only
        source = _notificationsRepository.Table.Where(x => x.UserID == userId && x.CommentUserID != userId && x.IsRead == (int)NotificationStatus.Unread).OrderByDescending(x => x.ID).ToList();
      return source;
    }

    public string GetUserColor(int id)
    {
      string color;
      var user = _usersRepository.Table.Where(u => u.ID == id).Include(u => u.ItemColor).Include(u => u.Comments).FirstOrDefault();
      color = user?.ItemColor.ColorHex;
      return color;
    }

    public string GetUserGender(int id)
    {
      return _usersRepository.Table.Where(u => u.ID == id).FirstOrDefault().Gender.ToString();
    }

    public void ClearCommentNotif(int userId, int lastCommentNotificationID)
    {
      var source = _notificationsRepository.Table.Where(x => x.UserID == userId && x.IsRead == 0 && x.ID <= lastCommentNotificationID).ToList();
      source.ForEach(x =>
      {
        x.IsRead = 1;
        _notificationsRepository.Update(x);
      });
    }  // Not used for now

    public void ClickBell(int userId) // Mark all user notifications as Read if it is Unread
    {
      var source = _notificationsRepository.Table.Where(x => x.UserID == userId && x.IsRead == 0).ToList();
      source.ForEach(x =>
      {
        x.IsRead = 1; // NotificationStatus.Read
        _notificationsRepository.Update(x);
      });
    }

    public int ClearBrokenLinks() // Notifications.StoryId -> Stories.ID
    {
      int i = 0;
      var notifications = _notificationsRepository.Table.ToList();
      var stories = _repository.Table.ToList();
      notifications.ForEach(n =>
      {
        if (stories.Where(s => s.ID == n.StoryId).Count() <= 0)
        { _notificationsRepository.Delete(n); i++; }
      });
      return i;
    }

    // set notification status to Isread=2 (clicked)
    public Notification SetNotifStatus(int notification_id, byte isRead)
    {
      var source = _notificationsRepository.Table.Where(x => x.ID == notification_id).FirstOrDefault();
      if (source != null)
      {
        source.IsRead = isRead;
        _notificationsRepository.Update(source);
      }
      return source;
    }

    public bool IsSpam(int userId)
    {
      bool result = false;
      int approvalTimeLimit = Convert.ToInt32(_configuration["ApprovalTimeLimit"]);
      var posts = _repository.Table.Where(p => p.UserID == userId);
      if (posts.Count() > 0)
      {
        var lastPostDate = posts.OrderByDescending(t => t.PublishDate).First().PublishDate;
        if ((DateTime.Now - lastPostDate).TotalMinutes < approvalTimeLimit)
          result = true;
      }
      return result;
    }

    public int UpdateStatusPendingStories()
    {
      int result = 0;
      try
      {
        int approvalTimeLimit = Convert.ToInt32(_configuration["ApprovalTimeLimit"]);
        List<string> words = GetBadWordsCach();
        var allApprovedStoryUsers = _repository.Table.Where(p => p.StoryStatus == StoryStatus.Approved).OrderByDescending(d => d.PublishDate).DistinctBy(u => u.UserID).ToList();
        foreach (var user in allApprovedStoryUsers)
        {

          var pendingApprovalStories = _repository.Table.Where(p => p.UserID == user.UserID && p.StoryStatus == StoryStatus.PendingApproval).ToList();
          if (pendingApprovalStories != null && pendingApprovalStories.Count > 0)
          {
            foreach (var story in pendingApprovalStories)
            {
              var enforceMode = _configuration["BadWordsEnforceMode"].EnumTryParse(BadWordsEnforceMode.Unspecified);
              bool hasBadWords = words.Any(word => story.Body.ToLower().ContainsBadWord(word, enforceMode));
              if ((user.ApprovedDate <= story.PublishDate) && (DateTime.Now - story.PublishDate).TotalMinutes >= approvalTimeLimit && !hasBadWords)
              {
                story.StoryStatus = StoryStatus.Approved;
                story.ApprovedDate = DateTime.Now;
                _repository.Update(story);
                AddStoryApprovedNotification(story, false);
                result = 1;
              }
            }
          }
        }

        return result;
      }
      catch (Exception)
      {
        return result;
      }
    }

    public User GetBlockedHiddenReported(int? uId, BlockedRule blockedRule)
    {
      if (uId == null) return null;
      var blockedUsers = _usersRepository.TableNoTracking
          .Include(p => p.HidenUser).Include(d => d.BlockedUser).Include(r => r.ReportedUsers).First(p => p.ID == uId);
      switch (blockedRule)
      {
        case BlockedRule.ForPosts:
          blockedUsers.HidenUser = _hiddenRepository.TableNoTracking.Where(h => h.LeftId == uId).ToList();
          break;
        case BlockedRule.ForUsers:
          blockedUsers.HidenUser = _hiddenRepository.TableNoTracking.Where(h => h.RightId == uId).ToList();
          break;
        case BlockedRule.ForMessages:
          blockedUsers.HidenUser = _hiddenRepository.TableNoTracking.Where(h => h.LeftId == uId || h.RightId == uId).ToList();
          break;
        default:
          blockedUsers.HidenUser = _hiddenRepository.TableNoTracking.Where(h => h.LeftId == uId).ToList();
          break;
      }
      blockedUsers.BlockedUser = _blockedRepository.TableNoTracking.Where(b => b.LeftId == uId || b.RightId == uId).ToList();
      blockedUsers.ReportedUsers = _reportRepository.TableNoTracking.Where(r => r.LeftId == uId || r.RightId == uId).ToList();
      return blockedUsers;
    }

    public List<User> GetNewStoryAlertReceivers(Story s, User blockedUsers)
    {
      var user = _customerService.GetUserById(s.UserID); //User posted story
      if (s == null && user.Deactivated == true && s.StoryStatus != StoryStatus.Approved)
        return null;

      var langu = _languagesRepository.GetById(user.LanguageId);
      var logedUserAge = Domain.Helpers.DateHelper.GetAge(user.YearOfBirth);
      var ageGroup = Domain.Helpers.DateHelper.GetAgeGroup(logedUserAge);
      List<int> usersByAge = _usersRepository.TableNoTracking.Where(p =>
          Domain.Helpers.DateHelper.GetAge(p.YearOfBirth) >= ageGroup.Key &&
          Domain.Helpers.DateHelper.GetAge(p.YearOfBirth) <= ageGroup.Value).Select(p => p.ID).ToList();

      var posts = (from post in _repository.Table
                   where
                       post.User.Deactivated != true
                       && usersByAge.Contains(post.UserID)
                       && post.User.CountryId == (user.CountryId)
                       && post.User.LanguageId == (user.LanguageId)
                       && (post.Language == (user.LanguageId == 1 ? "he" : "en")
                       || (langu.Iso.ToUpper() == post.Language.ToUpper() || post.UserID == s.UserID))
                   select post).AsQueryable();

      posts = posts.Where(post =>
          //Hidden user. When UserA hides UserB: UserA can't see UserB in feed but UserB can see UserA
          !blockedUsers.HidenUser.Any(h => h.LeftId == post.UserID)
          //Blocked user. When UserA blocked UserB: UserA and UserB can't see each other (reqs changed)
          && !blockedUsers.BlockedUser.Any(b => b.RightId == post.UserID && b.LeftId == s.UserID)
          && !blockedUsers.BlockedUser.Any(b => b.LeftId == post.UserID && b.RightId == s.UserID)
          //Reported user. When UserA reported UserB: UserA and UserB can't see each other
          && !blockedUsers.ReportedUsers.Any(r => r.RightId == post.UserID && r.LeftId == s.UserID)
          && !blockedUsers.ReportedUsers.Any(r => r.LeftId == post.UserID && r.RightId == s.UserID)
      );

      return posts.Select(p => p.User).Distinct().ToList();

    }

    public List<string> GetBadWordsCach()
    {
      List<string> words = new List<string>();
      if (!cacheBadWords.TryGetValue("BadWords", out words))
      {
        words = _wordRepository.Table.Select(x => x.Value.ToLower()).ToList();
        cacheBadWords.Set("BadWords", words,
        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));
      }
      return words;
    }

    public List<StoryModelCron> CheckPendingStatus() // Spam Autoapproval
    {
      int approvalTimeLimit = Convert.ToInt32(_configuration["ApprovalTimeLimit"]);
      List<StoryModelCron> models = new List<StoryModelCron>();
      //var words = _wordRepository.Table.Select(x => x.Value.ToLower()).ToList();
      List<string> words = GetBadWordsCach();
      var enforceMode = _configuration["BadWordsEnforceMode"].EnumTryParse(BadWordsEnforceMode.Unspecified);
      var posts = _repository.Table.Where(p => p.StoryStatus == StoryStatus.PendingApproval
              && !words.Any(word => p.Body.ToLower().ContainsBadWord(word, enforceMode))
              && (DateTime.Now - p.PublishDate).TotalMinutes >= approvalTimeLimit)
          .OrderBy(d => d.PublishDate).DistinctBy(u => u.UserID).ToList();
      posts.ForEach(s =>
      {
        int apprPost = _repository.Table.Where(p => p.UserID == s.UserID
                  && p.StoryStatus == StoryStatus.Approved
                  && (DateTime.Now - p.ApprovedDate).TotalMinutes < approvalTimeLimit).Count();
        if (apprPost <= 0)
        {
          s.StoryStatus = StoryStatus.Approved;
          s.ApprovedDate = DateTime.Now;
          _repository.Update(s);
          List<Notification> notifs = new List<Notification>();
          notifs.Add(AddStoryApprovedNotification(s, false));

          User blockedUsers = GetBlockedHiddenReported(s.UserID, BlockedRule.ForUsers);
          var userList = GetNewStoryAlertReceivers(s, blockedUsers);

          StoryModelCron model = new StoryModelCron
          {
            storyData = GetStoryModel(s.ID, s.UserID, blockedUsers),
            receivers = userList,
            notificationData = notifs
          };
          models.Add(model);
        }
      });
      return models;
    }

    public List<StoryModelCron> GetApprovedStories() // Approved from Admin APP
    {
      List<StoryModelCron> models = new List<StoryModelCron>();
      List<int> storiesList = GetApprovedByAdmin();
      storiesList.ForEach(x =>
      {
        StoryModelCron storyModelCron = SendStoryApproved(x);
        if (storyModelCron != null)
          models.Add(storyModelCron);
      });
      return models;
    }

    public StoryModelCron SendStoryApproved(int sId) // Approved from Admin APP
    {
      Story s = GetStoryById(sId);
      if (s.StoryStatus != StoryStatus.Approved)
        return null;

      User blockedUsers = GetBlockedHiddenReported(s.UserID, BlockedRule.ForUsers);
      var userList = GetNewStoryAlertReceivers(s, blockedUsers);

      if (userList != null)
      {
        var lastNotifications = _notificationsRepository?.Table.Where(c => c.StoryId == sId && c.IsRead == (int)NotificationStatus.Unread);
        lastNotifications = lastNotifications.Where(c => c.Type == (int)NotificationType.STORY_APPROVED || c.Type == (int)NotificationType.STORY_PENDING).OrderByDescending(c => c.ID).Take(2);

        StoryModelCron storyModelCron = new StoryModelCron()
        {
          storyData = GetStoryModel(sId, s.UserID, blockedUsers),
          receivers = userList,
          notificationData = lastNotifications.Count() > 0 ? lastNotifications.ToList() : null
        };
        return storyModelCron;
      }
      return null;
    }

    public List<int> GetApprovedByAdmin()
    {
      return _notificationsRepository.Table.Where(s => s.ByAdmin == true && s.Type == (int)NotificationType.STORY_APPROVED).Select(i => i.StoryId).ToList();
    }

    public void ResetFlagChangedByAdmin(NotificationType notificationType)
    {
      var notifs = _notificationsRepository.Table.Where(s => s.ByAdmin == true && s.Type == (int)notificationType).ToList();
      notifs.ForEach(x =>
      {
        x.ByAdmin = false;
        _notificationsRepository.Update(x);
      });
    }

    // Scenario
    // UserB Create 2 Stories
    // UserA Tail UserB and UserC
    // UserB Create 2 Stories, UserC Create 3 Stories
    // Result: Total count should be 5 for userA
    public int NewImTailingCount(int uId)
    {
      int count = 0;
      //var tailedStoryes = _tailedStoryesRepository.Table.Where(t => t.uId == uId).ToList();
      //tailedStoryes.ForEach(s =>
      //{
      //    var story = _repository.Table.Join(_tailedUsersRepository.Table, p => p.UserID, q => q.RightId, (p, q) => new { p, q })
      //        .Where(p => p.q.LeftId == s.uId && p.p.ID == s.sId && p.p.ApprovedDate > p.q.TailedDate);
      //    if (story.Count() > 0) count++;
      //});

      var tStoryes = (from ts in _tailedStoryesRepository.Table
                      join s in _repository.Table on ts.sId equals s.ID
                      join tu in _tailedUsersRepository.Table on s.UserID equals tu.RightId
                      where ts.uId == uId && tu.LeftId == uId
                      select new
                      {
                        ts.uId,
                        ts.sId,
                        s.ApprovedDate,
                        tu.TailedDate,
                        tu.LeftId,
                        tu.RightId
                      }
          ).Where(p => p.ApprovedDate > p.TailedDate).DistinctBy(p => p.sId);

      //tStoryes = tStoryes.Where(p => p.ApprovedDate > p.TailedDate).ToList();

      //tStoryes = tStoryes.DistinctBy(p => p.sId).ToList();

      //tStoryes = from ts in tStoryes where ts.ApprovedDate > ts.TailedDate select ts;

      count = tStoryes.Count();

      return count;
    }

    public PollModel GetPoll(int storyId)
    {
      var story = _repository.GetById(storyId);
      if (story == null)
        throw new InvalidOperationException("Story not found");
      if (string.IsNullOrEmpty(story.PollData))
        return null;
      return JsonConvert.DeserializeObject<PollModel>(story.PollData);
    }

    public void InsertOrdUpdatePoll(PollModel model, int userId)
    {
      Story s;
      if (model.StoryId == 0)
      {
        s = new Story
        {
          ApprovedDate = DateTime.Now,
          Body = "",
          Description = "",
          IsCommentsAlowed = false,
          PublishDate = DateTime.Now,
          StoryStatus = model.StoryStatus == Entities.Enums.StoryStatus.Draft ?
            Entities.Enums.StoryStatus.Draft : Entities.Enums.StoryStatus.PendingApproval,
          Title = model.Question,
          UserID = userId,
          StoryTopic = new List<StoryTopic>(),
          Category = model.Category,
          SubCategory = model.SubCategory,
          IsPoll = true,
          PollData = JsonConvert.SerializeObject(model),
          Language = model.Language
        };
        _repository.Insert(s);
      }
      else
      {
        s = _repository.GetById(model.StoryId);
        s.Title = model.Question;
        s.PollData = JsonConvert.SerializeObject(model);
        s.StoryStatus = Entities.Enums.StoryStatus.PendingApproval;
        _repository.Update(s);
      }
    }

  }

  public static class PostServiceExtensions
  {

    public static bool ContainsBadWord(this string currWord, string badWord)
    {
      return ContainsBadWord(currWord, badWord, BadWordsEnforceMode.Normal);
    }

    public static bool ContainsBadWord(this string currWord, string badWord, BadWordsEnforceMode enforceMode)
    {
      bool naive = currWord.Contains(badWord);
      if (naive)
        return true;

      if (enforceMode == BadWordsEnforceMode.Lenient)
        return false;

      if (enforceMode == BadWordsEnforceMode.Normal)
      {
        string[] x = badWord.Select(ch => ch.ToString()).ToArray();
        string word0 = string.Join('-', badWord.ToCharArray().ToString());   //dash-delimited
        string word1 = string.Join('.', badWord.ToCharArray().ToString());   //dot-delimited
        string word2 = string.Join(' ', x);   //space-delimited
        return currWord.Contains(word0) || currWord.Contains(word1) || currWord.Contains(word2);
      }

      var currWordLetters = currWord.Select(ch => ch.ToString()).ToArray();
      var badWordLetters = badWord.Select(ch => ch.ToString()).ToArray();
      int currStart = 0;
      for (int i = 0; i < badWordLetters.Length; i++)
      {
        bool isOK = false;
        for (int j = currStart; j < currWordLetters.Length; j++)
        {
          if (badWordLetters[i] == currWordLetters[j])
          {
            currStart = j + 1;
            isOK = true;
          }
        }

        if (!isOK)
          return false;
      }

      return true;
    }

    //  public static bool ContainsBadWord2(this string currWord, string badWord)
    //  {
    //    bool naive = currWord.Contains(badWord);
    //    if (naive)
    //      return true;

    //    string[] x = badWord.Select(ch => ch.ToString()).ToArray();
    //    string word0 = string.Join('-', badWord.ToCharArray().ToString());   //dash-delimited
    //    string word1 = string.Join('.', badWord.ToCharArray().ToString());   //dot-delimited
    //    string word2 = string.Join(' ', x);   //space-delimited
    //    return currWord.Contains(word0) || currWord.Contains(word1) || currWord.Contains(word2);
    //  }

  }

}
