using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL;
using Entities;
using Services.Notifications;
using Services.Posts;
using Services.Customers;
using WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Domain;
using Microsoft.Extensions.Localization;
using Entities.Enums;

namespace WebApi.Controllers
{
    //[Produces("application/json")]
    [Route("/api/v1/user")]
    public class NotificationsController : BaseControler
    {
        private readonly INotificationService _notificationService;
        private readonly IPostService _postService;
        private readonly IStagingRepository<Comment> _commentsRepository;
        private readonly ICustomerService _customerService;
        private readonly IStagingRepository<User> _usersRepository;
        private readonly IStringLocalizer<NotificationsController> _localizer;

        public NotificationsController(
            INotificationService notificationService,
            IPostService postService,
            IStagingRepository<Comment> commentsRepository,
            ICustomerService customerService,
            IStagingRepository<User> usersRepository,
            IStringLocalizer<NotificationsController> localizer)
        {
            _notificationService = notificationService;
            _postService = postService;
            _commentsRepository = commentsRepository;
            _customerService = customerService;
            _usersRepository = usersRepository;
            _localizer = localizer;
        }

        /// <summary>
        /// Get Notifications.
        /// Input: int page, bool read, int limit; Output: notifs_no, unread_notifs_no, List(NotificationModel)
        /// </summary>
        /// <typeparam name = "page"> int </typeparam>
        /// <typeparam name = "read"> bool </typeparam>
        /// <typeparam name = "limit"> int </typeparam>
        /// <returns> notifs_no, unread_notifs_no, List(NotificationModel) </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        //[Authorize]
        [Route("Notifications")]
        [HttpGet]
        public object GetNotifications(int page = 1, bool read = false, int limit = 3)
        {
            try
            {
                int notifs_no, unread_notifs_no;
                List<NotificationModel> notificationsOnPage = new List<NotificationModel>();
                var notifications = _postService.GetNewNotifications(UserId, read).Distinct().OrderByDescending(d => d.NotifDate);
                if (notifications == null || notifications.Count() == 0)
                {
                    notificationsOnPage = null;
                    unread_notifs_no = 0;
                    notifs_no = 0;
                }
                else
                {
                    notifs_no = notifications.Count();
                    unread_notifs_no = notifications.Where(x => x.IsRead == (int)NotificationStatus.Unread).Count();

                    if (read)
                        notificationsOnPage = notifications.Skip((page - 1) * limit).Take(limit)
                            .Select(n => new NotificationModel(n, _customerService, _postService, _localizer, _commentsRepository)).ToList();
                    else
                        notificationsOnPage = notifications.Where(x => x.IsRead == (int)NotificationStatus.Unread).Skip((page - 1) * limit).Take(limit)
                            .Select(n => new NotificationModel(n, _customerService, _postService, _localizer, _commentsRepository)).ToList();
                }

                return CreateResponse(new
                {
                    notifs_no,
                    unread_notifs_no,
                    notifs_on_this_page = notificationsOnPage
                });
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Update Comment Status (PendingApproval, DeniedApproval).
        /// Input: Comment Object; Output: CommentModel Object
        /// </summary>
        /// <typeparam name = "comment"> Comment </typeparam>
        /// <returns> CommentModel Object </returns>
        /// <exception cref = "AdminRightsRequired"> Admin Rights Required </exception>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Authorize]
        [Route("UpdateCommentStatus")] // It should be in the CommentsController
        [HttpPut]
        public object UpdateCommentStatus([FromBody]Entities.Comment model) // update PendingApproval, DeniedApproval
        {
            if (!_customerService.IsAdmin(UserId))
                return Ok(CreateResponse(model, false, new Error[1] { new Error("Admin", _localizer["AdminRightsRequired"]) }));

            try
            {
                _postService.UpdateCommentStatus(model, false);
                //sends comment notification email
                //if (model.IsPendingApproval == true)
                //    await SendPostNotificationEmailAsync(model, CommentNotificationType.USER_COMMENTED);
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(model, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }

            return CreateResponse(model);
        }

        ///// <summary>
        ///// Clear Notififications (Mark as read) - Not used ?.
        ///// Input: int lastNotificationID; Output: int User ID
        ///// </summary>
        ///// <typeparam name = "lastNotificationID"> int </typeparam>
        ///// <returns> UserId </returns>
        ///// <exception cref = "ApplicationError"> DataBase Error </exception>
        //[Authorize]
        //[HttpGet]
        //[Route("ClearNewNotif")] // not used?
        //public object ClearNotif(int lastNotificationID)
        //{
        //    try
        //    {
        //        _postService.ClearCommentNotif(UserId, lastNotificationID);
        //        return Ok(CreateResponse(UserId, success: true));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
        //    }
        //}

        /// <summary>
        /// Set notification status to IsRead {Unread = 0, Read = 1, Clicked = 2}.
        /// Input: notification id, isRead; Output: NotificationModel Object
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <typeparam name = "isRead"> byte </typeparam>
        /// <returns> NotificationModel Object </returns>
        /// <exception cref = "DoesntExistInDB"> Does not exist in the database </exception>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Authorize]
        [HttpPut]
        [Route("{id}/SetNotifStatus")]
        public object SetNotifStatus(int id, byte isRead) // set notification status to Isread=2 (clicked)
        {
            try
            {
                var notif = _postService.SetNotifStatus(id, isRead);
                if (notif == null)
                    return Ok(CreateResponse(null, false, new Error[1] { new Error("DoesntExistInDB", _localizer["DoesntExistInDB"]) }));
                int userNotif = notif.CommentUserID; //return user who placed comment
                if (notif.Type != (int)NotificationType.USER_COMMENTED && notif.Type != (int)NotificationType.USER_LIKED)
                    userNotif = notif.UserID; //return post owner
                var model = new NotificationModel(notif, _customerService, _postService, _localizer, _commentsRepository);

                return Ok(CreateResponse(model, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }


        [Authorize]
        [HttpPut]
        [Route("ClickBell")]
        public object ClickBell() //Mark all user notifications as Clicked if it is Unread
        {
            try
            {
                _postService.ClickBell(UserId); 
                return Ok(CreateResponse(UserId, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("ClearBrokenLinks")]
        public object ClearBrokenLinks()
        {
            try
            {
                int qty = _postService.ClearBrokenLinks();
                return Ok(CreateResponse(qty, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

    }
}