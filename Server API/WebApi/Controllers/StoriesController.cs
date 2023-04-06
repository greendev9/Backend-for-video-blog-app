using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using Domain.Helpers;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Services.Customers;
using Services.Localization;
using Services.Messages;
using Services.Posts;
using SQLitePCL;
using WebApi.Extensions;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("/api/v1/stories")]
    [Authorize]
    public class StoriesController : BaseControler
    {
        private readonly IPostService _postService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IStagingRepository<Comment> _commentsRepository;
        private readonly IMapper _mapper;
        private readonly IStagingRepository<User> _usersRepository;
        private readonly IMessageService _messageService;
        private readonly IStringLocalizer<StoriesController> _localizer;
        private readonly IConfiguration _configuration;

        public StoriesController(IPostService postService, 
        ILocalizationService localService, 
        ICustomerService customerService, 
        IStagingRepository<Comment> commentsRepository,
        IMapper mapper, 
        IStagingRepository<User> usersRepository,
        IMessageService messageService,
        IStringLocalizer<StoriesController> localizer,
        IConfiguration configuration
        )
        {
            _postService = postService;
            _localizationService = localService;
            _customerService = customerService;
            _commentsRepository = commentsRepository;
            _mapper = mapper;
            _usersRepository = usersRepository;
            _messageService = messageService;
            _localizer = localizer;
            _configuration = configuration;
        }

        /// <summary>
        /// Get Stories.
        /// Input: int page, int limit, string q, int status, int timeMode, string catalog, string subCatalog, string topics, bool followersOnly; Output: IEnumerable(PostPresentationModel)
        /// </summary>
        /// <typeparam name = "page"> StoryModel Class </typeparam>
        /// <typeparam name = "limit"> StoryModel Class </typeparam>
        /// <typeparam name = "q"> StoryModel Class </typeparam>
        /// <typeparam name = "status"> StoryModel Class </typeparam>
        /// <typeparam name = "timeMode"> StoryModel Class </typeparam>
        /// <typeparam name = "catalog"> StoryModel Class </typeparam>
        /// <typeparam name = "subCatalog"> StoryModel Class </typeparam>
        /// <typeparam name = "topics"> StoryModel Class </typeparam>
        /// <typeparam name = "followersOnly"> StoryModel Class </typeparam>
        /// <returns> IEnumerable(PostPresentationModel) </returns>
        [HttpGet]
        public object Get(int page = 0, int limit = 10, string q = null, int status = 1, int timeMode = 0, string catalog = null, 
            string subCatalog = null, string topics = null, bool followersOnly = false)
        {
            List<int> stopic = null;
            if (!string.IsNullOrEmpty(topics))
            stopic = topics.Split(',').Select(x => int.Parse(x)).ToList();
            try
            { 
                var result = _postService.SearchStories(
                    OnlyMine: !string.IsNullOrEmpty(Request.Query["Me"]),
                    SkipFilters: !string.IsNullOrEmpty(Request.Query["Me"]),
                    status: (StoryStatus)status,
                    Title: q,
                    dateFilter: (Domain.Models.Posts.DateFilterType)timeMode,
                    UserId: UserId,
                    Catalog: catalog,
                    topics: stopic,
                    SubCatalog: subCatalog,
                    pageSize: limit,
                    includeUser: false,
                    skip: page * limit,
                    OnlyFromFollowingUsers: followersOnly,
                    skipCatalogs: string.IsNullOrEmpty(catalog)
                );
                return CreateResponse(result);
                }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Insert Story, Generate and Insert Notifications.
        /// Input: StoryModel Class; Output: [UserInfo Class, List(NotificationModel), StoryModel Class]
        /// </summary>
        /// <typeparam name = "model"> StoryModel Class </typeparam>
        /// <returns> [UserInfo Class, List(NotificationModel), StoryModel Class] </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
	    [HttpPost]
        public async Task<object> Post([FromBody] StoryModel model)
        {
            if (model.Category == null && (model.Title == null || String.IsNullOrEmpty(model.Title.Trim())))
                ModelState.AddModelError("Title", _localizer["StoryEmptyTitle"]);

            if (ModelState.IsValid)
            {
                var spbod = model.Body.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                var storyBody = string.Empty;
                bool status = ModelState.IsValid;
                foreach (var item in spbod)
                    storyBody += "<p>" + item + "</p>";
                model.Body = storyBody;

                int queryMaxLength = 900; // http://api.languagelayer.com service restriction on query length (1000 symbols max)
                string query = model.Body.Length > queryMaxLength ? model.Body.Substring(0, queryMaxLength) : model.Body;
                if (!String.IsNullOrEmpty(query))
                {
                    try { model.Language = await _localizationService.DetectLangaugeAsync(query.Trim()); }
                    catch (Exception ex)
                    {
                        return Ok(CreateResponse(model, false, new Error[1] { new Error("LangDetectorError", _localizer["LangDetectorError"] + " " + ex.Message) }));
                    }
                }
                else
                    model.Language = "EN";
                if (String.IsNullOrEmpty(model.Language))
                    return Ok(CreateResponse(model, false, new Error[1] { new Error("LangDetectorError", _localizer["LangDetectorError"]) }));

                //List<string> words = new List<string>();
                //if (!cacheBadWords.TryGetValue("BadWords", out words))
                //{
                //    words = _customerService.GetAllWords().Select(x => x.Value.ToLower()).ToList();
                //    cacheBadWords.Set("BadWords", words,
                //    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));
                //}

                List<string> words = _postService.GetBadWordsCach();

                StoryStatus newStatus = model.StoryStatus == StoryStatus.Draft ? StoryStatus.Draft : StoryStatus.PendingApproval;
        
                Story s;
                s = new Story
                {
                    ApprovedDate = DateTime.Now,
                    Body = model.Body,
                    Description = model.Description,
                    IsCommentsAlowed = model.IsCommentsAlowed,
                    PublishDate = DateTime.Now,
                    StoryStatus = model.StoryStatus,
                    Title = model.Title,
                    UserID = UserId,
                    StoryTopic = model.StoryTopics?.Select(item => new StoryTopic() { RightId = item }).ToList(),
                    Language = model.Language,
                    Category = model.Category, // Catagory = NULL ( Home page feed - STORY); If Category = {travel, food, startup} (Community - POST).
                    SubCategory = model.SubCategory
                };

                if (newStatus != StoryStatus.Draft)
                {
                    model.IsPending = true;
                    if (!_postService.IsSpam(UserId))
                    {
                        var enforceMode = _configuration["BadWordsEnforceMode"].EnumTryParse(BadWordsEnforceMode.Unspecified);
                        bool hasBadWords = words.Any(word => storyBody.ToLower().ContainsBadWord(word, enforceMode));
                        if (!hasBadWords)     //Note: Auto approval feature.
                        {
                            s.StoryStatus = StoryStatus.Approved;
                            s.ApprovedDate = DateTime.Now;
                            model.IsPending = false;
                        }
                    }
                }

                try
                {
                    var notifs = _postService.InsertStory(s);
                    model.ID = s.ID;
                    model.StoryStatus = s.StoryStatus;
                    model.UserID = s.UserID;
                    var user = _customerService.GetUserById(UserId);
                    var userModel = new UserInfo(user, _postService, _customerService, _messageService);
                    List<NotificationModel> notifModels = notifs?.Select(n => new NotificationModel(n, _customerService, _postService, _localizer, _commentsRepository)).ToList();
                    return CreateResponse(new { user = userModel, notification = notifModels, model });
                }
                catch (Exception ex)
                {
                    return Ok(CreateResponse(model, false, new WebApi.Models.Error[1] { new WebApi.Models.Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
                }
            }
            else
            {
                return CreateResponse(model, ModelState.IsValid, ModelState.GetErrors());
            }
        }
        
        /// <summary>
         /// Update Story, Generate and Insert Notifications.
         /// Input: StoryModel Class; Output: [UserInfo Class, List(NotificationModel), StoryModel Class]
         /// </summary>
         /// <typeparam name = "model"> StoryModel Class </typeparam>
         /// <returns> [UserInfo Class, List(NotificationModel), StoryModel Class] </returns>
         /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [HttpPut]
        public object Put([FromBody] StoryModel model)
        {
            if (model.ID <= 0)
                ModelState.AddModelError("ID", _localizer["IdRequred"]);

            var storyBody = string.Empty;
            if (model.Body != null)
            {
                var spbod = model.Body.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in spbod)
                    storyBody += "<p>" + item + "</p>";
                model.Body = storyBody;
            }
            List<string> words = _postService.GetBadWordsCach();


            StoryStatus newStatus = new StoryStatus();
            if (model.StoryStatus == StoryStatus.Draft)
                newStatus = StoryStatus.Draft;
            else if (model.StoryStatus == StoryStatus.NotApproved)
                newStatus = StoryStatus.NotApproved;
            else
                newStatus = StoryStatus.PendingApproval;

            if (ModelState.IsValid)
            {
    		    try
		    {
			    Story story = _postService.GetStoryById(model.ID);
                if (story == null)
                    return CreateResponse(null, false, new Error[1] { new Error("Access", _localizer["StoryNotFound"]) });
                if (story.UserID != UserId && !_customerService.IsAdmin(UserId))
                return CreateResponse(null, false, new Error[1] { new Error("Access", _localizer["AuthorOrAdminOnly"]) });

                if (model.UserID <= 0)
                    model.UserID = story.UserID;

                if (newStatus != StoryStatus.Draft && newStatus != StoryStatus.NotApproved)
                {
                    model.IsPending = true;
                    var enforceMode = _configuration["BadWordsEnforceMode"].EnumTryParse(BadWordsEnforceMode.Unspecified);
                    bool hasBadWords = words.Any(word => storyBody.ToLower().ContainsBadWord(word, enforceMode));
                    if (!hasBadWords) //Note: Auto approval feature.
                    {
                        model.StoryStatus = StoryStatus.Approved;
                        model.IsPending = false;
                    }
                }

         
                if (story.StoryStatus == StoryStatus.NotApproved && model.StoryStatus == StoryStatus.Approved)
                {
                    model.StoryStatus = StoryStatus.PendingApproval;
                    model.IsPending = true;
                }

                // if status changed to Approved from any other
                if (model.StoryStatus == StoryStatus.Approved && story.StoryStatus != StoryStatus.Approved)
                    story.PublishDate = DateTime.Now;

                _mapper.Map<StoryModel, Story>(model, story);
			    story.IsCommentsAlowed = model.IsCommentsAlowed;
                        
                List<Notification> notifs = _postService.UpdateStory(story);
                var user = _customerService.GetUserById(UserId);
                var userModel = new UserInfo(user, _postService, _customerService, _messageService);

                model = new StoryModel(story);
                List<NotificationModel> notifModels = notifs?.Select(n => new NotificationModel(n, _customerService, _postService, _localizer, _commentsRepository)).ToList();

                return CreateResponse(new { user = userModel, notification = notifModels, model });
            }
            catch (Exception ex)
		    {
				    return Ok(CreateResponse(model, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
		    }
	        }
            else
            {
                return CreateResponse(model, false, ModelState.GetErrors());
            }
        }

        /// <summary>
        /// Deny Story, Generate and Insert Notifications.
        /// Input: int Story ID; Output: success: true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success: true/false </returns>
        /// <exception cref = "IdRequred"> Story Id is requred </exception>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        /// <exception cref = "PostNotFound"> Post was not found Error </exception>
        [Route("{id}/story")]
        [HttpPut]
        public object DenyStory(int id)
        {
            if (!_customerService.IsAdmin(UserId))
                return Ok(CreateResponse(null, false, new Error[1] { new Error("Admin", _localizer["AdminRightsRequired"]) }));
            var post = _postService.GetStoryById(id);
            if (post != null)
            {
                try
                {
                    List<Notification> notif = _postService.DenyStory(UserId, id);
                    return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
                }
                catch (Exception ex)
                {
                    return Ok(CreateResponse(success: false, data: _localizer["HasNoResponse"].Value, errors: new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
                }
            }
            else
                return CreateResponse(null, false, new List<Error> { new Error("id", _localizer["PostNotFound"]) });
        }

        /// <summary>
        /// Like Story, Generate and Insert Notifications.
        /// Input: int Story ID; Output: success: true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success: true/false </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        /// <exception cref = "PostNotFound"> Post was not found Error </exception>
        [Route("{id}/like")]
        [HttpPost]
        public object Like(int id)
        {
            var post = _postService.GetStoryById(id);
            if (post != null)
            {
                try
                {
                    Notification notif = _postService.LikeStory(UserId, id);
                    return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
                }
                catch (Exception ex)
                {
                    return Ok(CreateResponse(success: false, data: _localizer["HasNoResponse"].Value, errors: new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
                }
            }
            else
                return CreateResponse(null, false, new List<Error> { new Error("id", _localizer["PostNotFound"]) });
        }

        /// <summary>
        /// Get Story By Id.
        /// Input: int Story ID; Output: PostPresentationModel
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> PostPresentationModel Object </returns>
        /// <exception cref = "PostNotFound"> Post was not found Error </exception>
        [Route("{id}/story")]
        [HttpGet]
        public object Story(int id)
        {
            var post = _postService.GetStoryById(id);
            if (post != null)
            {
                var blockedUsers = _postService.GetBlockedHiddenReported(UserId, BlockedRule.ForPosts);
                var result = _postService.GetStoryModel(id, UserId, blockedUsers); //fix 3rd argument ???
                return Ok(CreateResponse(result, success: true));
            }
            else
                return CreateResponse(null, false, new List<Error> { new Error("id", _localizer["PostNotFound"]) });
        }

        /// <summary>
        /// Add Story as Viewed by me; Increase Story Views counter.
        /// Input: int Story ID; Output: success = true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "PostNotFound"> Post was not found Error </exception>
        [Route("{id}/view")]
        [HttpPost]
        public object View(int id)
    {
      var post = _postService.GetStoryById(id);
      if (post != null)
        _postService.ViewStory(UserId, id);
      else
        return CreateResponse(null, false, new List<Error> { new Error("id", _localizer["PostNotFound"]) });
      return Ok();
    }

        /// <summary>
        /// Physically delete Story.
        /// Input: int Story ID; Output: UserInfo Object
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> UserInfo Object </returns>
        /// <exception cref = "PostNotFound"> Post was not found Error </exception>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Route("{id}")]
        [HttpDelete]
        public object Delete(int id)
    {
        var story = _postService.GetStoryById(id);
        if (story == null || story.UserID != UserId)
            return CreateResponse(null, false, new Error[1] { new Error("Delete", _localizer["RecordNotFound"]) });
        try
        { 
            _postService.DeleteStory(id);

            var user = _customerService.GetUserById(UserId);
            var userModel = new UserInfo(user, _postService, _customerService, _messageService);
            return Ok(CreateResponse(new { user = userModel }));
        }
		catch (Exception ex)
		{
			return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
		}
    }

        /// <summary>
        /// Toggle comments is comments allowed or not.
        /// Input: int Story ID; Output: success = true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "PostNotFound"> Post was not found Error </exception>
        [Route("{id}/togglecomments")]
        [HttpPost]
        public object ToggleComments(int id)
    {
      var post = _postService.GetStoryById(id);
      if (post != null)
      {
        _postService.ToggleComments(id);
        return Ok();
      }
      return NotFound();
    }

        /// <summary>
        /// Get Stories Only From Following Users.
        /// Input: int page, int limit; Output: IEnumerable(PostPresentationModel)
        /// </summary>
        /// <typeparam name = "page"> StoryModel Class </typeparam>
        /// <typeparam name = "limit"> StoryModel Class </typeparam>
        /// <returns> IEnumerable(PostPresentationModel) </returns>
        [HttpGet]
        [Route("BlaBlog")]
        public object BlaBlog(int page = 0, int limit = 100)
    {
      var stories = _postService.SearchStories(UserId: UserId,
                          OnlyFromFollowingUsers: true, pageSize: limit, skip: page * limit).ToList();
      return CreateResponse(stories);
    }
  }
}