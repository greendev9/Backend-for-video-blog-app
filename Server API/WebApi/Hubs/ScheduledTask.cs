using Domain;
using Domain.Models.Posts;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Services.Customers;
using Services.Messages;
using Services.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Controllers;
using WebApi.Models;

namespace WebApi.Hubs
{
    public class ScheduledTask
    {
        private readonly IPostService _postService;
        private readonly ICustomerService _customerService;
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IStagingRepository<Comment> _commentsRepository;
        private readonly IStagingRepository<Notification> _notificationRepository;
        private readonly IStagingRepository<User> _userRepository;
        private readonly IStringLocalizer<NotificationsController> _localizerNotifications;
        private readonly IStagingRepository<BlockedUser> _blockedRepository;

        public ScheduledTask(IPostService postService,
            ICustomerService customerService,
            IMessageService messageService,
            IHubContext<ChatHub> hubContext,
            IStagingRepository<Comment> commentsRepository,
            IStagingRepository<Notification> notificationRepository,
            IStagingRepository<User> userRepository,
            IStringLocalizer<NotificationsController> localizerNotifications,
            IStagingRepository<BlockedUser> blockedRepo
        )
        {
            _postService = postService;
            _customerService = customerService;
            _messageService = messageService;
            _hubContext = hubContext;
            _commentsRepository = commentsRepository;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _localizerNotifications = localizerNotifications;
            _blockedRepository = blockedRepo;
        }

        public void Check()
        {
            //// Spam autoapproving mechanism (CRON). Approve automatically and send info to users through SignalR
            // CheckPendingStatus() is looking for stories which ready to be approved automatically according Spam rules
            List<StoryModelCron> spamApprovalModels = _postService.CheckPendingStatus();
            //// Admin changed story status (CRON). Find approved on Admin APP and send info to users through SignalR
            // Find all notifs with status ByAdmin = true (Admin approved)
            List<StoryModelCron> adminApprovalModels = _postService.GetApprovedStories();
            // Merging collections without duplicates
            List<StoryModelCron> commonList = spamApprovalModels.Union(adminApprovalModels).ToList();

            commonList.ForEach(async m =>
            {
                //User reciever = _customerService.GetUserById(m.storyData.UserId); //story owner
                foreach (var reciever in m.receivers)
                {
                    StoryModelHub result = new StoryModelHub()
                    {
                        storyData = m.storyData,
                        userInfo = new UserInfo(reciever, _postService, _customerService, _messageService),
                        notificationData = m.storyData.UserId == reciever.ID ?
                            m.notificationData?.Select(n => new NotificationModel(n, _customerService, _postService, _localizerNotifications, _commentsRepository)).ToList() : null
                    };
                    //to ReceiveApprovedStrory method
                    await _hubContext.Clients.User(reciever.Email).SendAsync("ReceiveApprovedStrory", result);
                }
            });
            //Set ByAdmin = false after info (Admin approved) been sent to users
            if (adminApprovalModels.Count() > 0)
                _postService.ResetFlagChangedByAdmin(NotificationType.STORY_APPROVED);

            //Check notifications from Admin APP (Admin approved story)
            var adminNotifs = _notificationRepository.Table.Join(_userRepository.Table, p => p.UserID, q => q.ID, (p, q) => new { p, q })
                .Where(p => p.p.ByAdmin == true);
            adminNotifs.Where(p => p.p.Type == (int)NotificationType.STORY_APPROVED).ToList().ForEach(async n =>
            {
                var notificationData = new NotificationModel(n.p, _customerService, _postService, _localizerNotifications, _commentsRepository);
                NotificationModelHub result = new NotificationModelHub() { notificationData = notificationData, userInfo = new UserInfo(n.q, _postService, _customerService, _messageService) };
                await _hubContext.Clients.User(n.q.Email).SendAsync("ReceiveNotification", result);
            });
            //Set ByAdmin = false after info (Admin rejected) been sent to users
            if (adminNotifs.Count() > 0)
                _postService.ResetFlagChangedByAdmin(NotificationType.STORY_REJECTED);

            //Check notifications from Admin APP (Admin approved comment)
            adminNotifs.Where(p => p.p.Type == (int)NotificationType.USER_COMMENTED).ToList().ForEach(async n =>
            {
                var notificationData = new NotificationModel(n.p, _customerService, _postService, _localizerNotifications, _commentsRepository);
                NotificationModelHub result = new NotificationModelHub() { notificationData = notificationData, userInfo = new UserInfo(n.q, _postService, _customerService, _messageService) };
                await _hubContext.Clients.User(n.q.Email).SendAsync("ReceiveNotification", result);
            });
            //Set ByAdmin = false after info been sent to story owner
            if (adminNotifs.Count() > 0)
                _postService.ResetFlagChangedByAdmin(NotificationType.USER_COMMENTED);

            // User blocked by Admin should be kicked-out from the app immediately
            var blockedUsersByAdmin = _postService.GetListBlockedByAdmin().Where(d=>(DateTime.Now - d.BlockDate).Minutes < 5).ToList();
            blockedUsersByAdmin.ForEach(async x =>
            {
                var blockedUserEmail = _customerService.GetUserEmail(x.RightId);
                await _hubContext.Clients.User(blockedUserEmail).SendAsync("ReceiveBlocked", new { userId = x.RightId, isBlockedByAdmin = true } );
                //_customerService.BlockedAlertSent(x);
            });
        }
    }
}
