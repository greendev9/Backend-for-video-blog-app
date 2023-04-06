using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Domain.Interfaces;
using Domain.Models.Notifications;
using Microsoft.Extensions.Configuration;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Customers;
using Services.Localization;
using Services.Notifications;
using Services.Posts;
using WebApi.Extensions;
using WebApi.Models.Messages;
using Domain;
using Domain.Helpers;
using Entities.Enums;
using Microsoft.Extensions.Localization;
using WebApi.Models;

namespace WebApi.Controllers
{

  [Authorize]
  [Microsoft.AspNetCore.Mvc.Route("/api/v1/[controller]")]
  public class CommentsController : BaseControler
  {

    private readonly IPostService _postService;
    private readonly ILocalizationService _localizationService;
    private readonly IMapper _mapper;
    private readonly ICustomerService _customerService;
    private readonly IConfiguration _configuration;
    private readonly INotificationService _notificationService;
    private INotificationTransport _emailTransport;
    private readonly IStagingRepository<User> _usersRepository;
    private readonly IStringLocalizer<CommentsController> _localizer;

        public CommentsController(IPostService postService,
            ILocalizationService localService,
            ICustomerService customerService,
            IMapper mapper,
            IConfiguration configuration,
            INotificationService notificationService,
            INotificationTransport emailTransport,
            IStagingRepository<User> usersRepository,
            IStringLocalizer<CommentsController> localizer)
        {
            _postService = postService;
            _localizationService = localService;
            _mapper = mapper;
            _customerService = customerService;
            _configuration = configuration;
            _notificationService = notificationService;
            this._emailTransport = emailTransport;
            _usersRepository = usersRepository;
            _localizer = localizer;
        }


        /// <summary>
        /// Get Comments of Story.
        /// Input: int Story ID, page, limit; Output: IEnumerable(CommentExtended)
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <typeparam name = "page"> int </typeparam>
        /// <typeparam name = "limit"> int </typeparam>
        /// <returns> IEnumerable(CommentExtended) </returns>
        [Route("{id}")]
        [HttpGet]
        public object Comments(int id, int page = 0, int limit = 100)
        {
            try
            { 
                var comments = _postService.LoadComments(id, page * limit, limit);
                return CreateResponse(comments);
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Post New Comment.
        /// Input: CommentModel Object; Output: CommentModel Object
        /// </summary>
        /// <typeparam name = "model"> CommentModel </typeparam>
        /// <returns> CommentModel Object </returns>
        /// <exception cref = "DataBaseError"> DataBase Error </exception>
        /// <exception cref = "CommentsNotAllowed"> Comments are not allowed </exception>
        /// <exception cref = "CommentsNotAllowedNotApproved"> Comments are not allowed. Story status is not approved </exception>
        [HttpPost]
        public object Post([FromBody]CommentModel model)
        {
            Story story = _postService.GetStoryById(model.StoryId);
            if (story == null)
            {
                return CreateResponse(model, false, new Error[1] { new Error("Comment", _localizer["StoryWasNotFound"]) });
            }
            if (!story.IsCommentsAlowed)
            {
                return CreateResponse(model, false, new Error[1] { new Error("Comment", _localizer["CommentsNotAllowed"]) });
            }
            if (story.StoryStatus != Entities.Enums.StoryStatus.Approved)
            {
                return CreateResponse(model, false, new Error[1] { new Error("Comment", _localizer["CommentsNotAllowedNotApproved"]) });
            }
            List<string> words = _postService.GetBadWordsCach();
            var comment = new Entities.Comment
            {
                Body = model.Body,
                PublishDate = DateTime.Now,
                StoryId = model.StoryId,
                UserId = UserId,
                IsPendingApproval = false
            };

            var enforceMode = _configuration["BadWordsEnforceMode"].EnumTryParse(BadWordsEnforceMode.Unspecified);
            bool hasBadWords = words.Any(word => model.Body.ContainsBadWord(word, enforceMode));
            if (hasBadWords)     //Note: Auto approval feature.
            {
                comment.IsPendingApproval = true;
                model.IsPending = true;
            }

            if (ModelState.IsValid)
            {
                bool isMyComment = UserId == story.UserID ? true : false;
                bool addNotification = !isMyComment;
                if (comment.IsPendingApproval == true)
                    addNotification = false;
                try
                {
                    _postService.InsertComment(comment, addNotification); // true - add notification
                    model.ID = comment.ID;
                    return CreateResponse(new { comment = model });
                }
                catch (Exception ex)
                {
                    return Ok(CreateResponse(model, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
                }

            }     
            else
            {
                return CreateResponse(comment, false, ModelState.GetErrors());
            }
        }

        private async Task<bool> SendPostNotificationEmailAsync(Comment comment, Entities.Enums.NotificationType type)
    {
        Story story = _postService.GetStoryById(comment.StoryId);
        User userPostOwner = _customerService.GetUserById(story.UserID);
        User userLeftComment = _customerService.GetUserById(UserId);
        string emailContentTempl = String.Empty;
        switch (type)
        {
            case Entities.Enums.NotificationType.USER_COMMENTED:
                emailContentTempl = Environment.CurrentDirectory + "/Content/CommentNotificationPending.txt";
                break;
        }
        var messageBuilder = new MessageBuilder(emailContentTempl);
        messageBuilder.AddToken(new MessageToken("NickName", userLeftComment.NickName));
        messageBuilder.AddToken(new MessageToken("Title", story.Title));
        messageBuilder.AddToken(new MessageToken("CommentBody", comment.Body));

        string body = await messageBuilder.BuildAsync();

        bool success = await _notificationService.SendNotificationAsync(new EmailNotification(
        _configuration["Smtp:from"],
        userPostOwner.Email,
        "Comment Notification",
        body));

        return success;
    }

        /// <summary>
        /// Update Comment.
        /// Input: CommentModel Object; Output: success = true/false
        /// </summary>
        /// <typeparam name = "model"> CommentModel </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "DataBaseError"> DataBase Error </exception>
        /// <exception cref = "CommentsNotAllowed"> Comments are not allowed </exception>
        /// <exception cref = "CommentsNotAllowedNotApproved"> Comments are not allowed. Story status is not approved </exception>
         [HttpPut]
        public object Put([FromBody]Comment model)
        {
            List<string> words = _postService.GetBadWordsCach();
            var enforceMode = _configuration["BadWordsEnforceMode"].EnumTryParse(BadWordsEnforceMode.Unspecified);
            bool hasBadWords = words.Any(word => model.Body.ContainsBadWord(word, enforceMode));
            if (hasBadWords)
            {
                model.IsPendingApproval = true;
            }
            model.PublishDate = model.PublishDate == DateTime.MinValue ? DateTime.Now : model.PublishDate;
            try
            { 
                _postService.UpdateComment(model);
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Delete Comment.
        /// Input: id (Comment ID); Output: success = true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success = true/false </returns>
        [HttpDelete]
        [Route("{id}")]
        public object Delete(int id)
        {
            try
            {
                bool result = _postService.RemoveComment(UserId, id);
                if (result)
                    return Ok(CreateResponse(_localizer["DeletedSucessfully"].Value, success: true));
                else
                    return Ok(CreateResponse(_localizer["CommentDeleteError"].Value, success: false));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }


    }
}