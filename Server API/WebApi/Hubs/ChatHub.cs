using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Services.Customers;
using Entities;
using Domain;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using Domain.Helpers;
using WebApi.Controllers;
using Microsoft.Extensions.Localization;
using Services.Posts;
using WebApi.Models;
using Entities.Enums;
using Services.Messages;
using Domain.Models.Posts;

namespace WebApi.Hubs
{
    //[Authorize]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private readonly IPostService _postService;
        private readonly IStagingRepository<Story> _storyRepository;
        private readonly IStagingRepository<MessageUser> _messageRepository;
        private readonly IStagingRepository<User> _userRepository;
        private readonly ICustomerService _customerService;
        private readonly IStringLocalizer<MessagesController> _localizerMessages;
        private readonly IStringLocalizer<NotificationsController> _localizerNotifications;
        private readonly IStagingRepository<Notification> _notificationRepository;
        private readonly IStagingRepository<Comment> _commentsRepository;
        private readonly IStagingRepository<TailedUsers> _tailedUsersRepository;
        private readonly IStagingRepository<LikedUserStory> _likesRepository;
        private readonly IStagingRepository<ItemColor> _colorRepository;
        private readonly IStagingRepository<Topic> _topicRepository;
        private readonly IStagingRepository<Language> _languagesRepository;
        private readonly IMessageService _messageService;
        private readonly IStagingRepository<BlockedUser> _blockedRepository;
        private readonly IStagingRepository<HidenUser> _hiddenRepository;
        private readonly IStagingRepository<ReportedUsers> _reportRepository;
        private readonly IConfiguration  _configuration;


        public ChatHub(
            IPostService postService,
            IStagingRepository<Story> storyRepo,
            IStagingRepository<MessageUser> _messageRepo,
            ICustomerService customerService,
            IStagingRepository<User> userRepository,
            IStringLocalizer<MessagesController> localizerMessages,
            IStringLocalizer<NotificationsController> localizerNotifications,
            IStagingRepository<Notification> notificationRepository,
            IStagingRepository<Comment> commentsRepository,
            IStagingRepository<TailedUsers> tailedUsers,
            IStagingRepository<LikedUserStory> likesRepository,
            IStagingRepository<ItemColor> colorRepository,
            IStagingRepository<Topic> topicRepositry,
            IStagingRepository<Language> langRepository,
            IMessageService messageService,
            IStagingRepository<BlockedUser> blockedRepo,
            IStagingRepository<HidenUser> hiddenRepository,
            IStagingRepository<ReportedUsers> reportRepository, 
            IConfiguration configuration
        )
        {
            _postService = postService;
            _storyRepository = storyRepo;
            _messageRepository = _messageRepo;
            _customerService = customerService;
            _userRepository = userRepository;
            _localizerMessages = localizerMessages;
            _localizerNotifications = localizerNotifications;
            _notificationRepository = notificationRepository;
            _commentsRepository = commentsRepository;
            _tailedUsersRepository = tailedUsers;
            _likesRepository = likesRepository;
            _colorRepository = colorRepository;
            _topicRepository = topicRepositry;
            _languagesRepository = langRepository;
            _messageService = messageService;
            _blockedRepository = blockedRepo;
            _hiddenRepository = hiddenRepository;
            _reportRepository = reportRepository;
            _configuration = configuration;
        }

        public async Task SendMessage(string toUserNick, string message)
        {
            var toUser = _userRepository.Table.Include(c => c.ItemColor).FirstOrDefault(u => u.NickName.ToLower() == toUserNick.ToLower().Trim());
            var UserId = Int32.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (toUser != null && UserId >= 0 && Context.UserIdentifier != toUser.Email) //if current user and reciever are not the same
            {
                string notAllowedResponse = String.Empty;

                var blockedUsers = _postService.GetBlockedHiddenReported(UserId, BlockedRule.ForMessages);

                bool notAllowed = _postService.IsUserBlockedMessages(UserId, toUser.ID, blockedUsers, out notAllowedResponse);
                notAllowedResponse = _localizerMessages[notAllowedResponse];

                MessageUser msg = new MessageUser
                {
                    IsRead = false,
                    LeftId = UserId, // from
                    MessageDate = DateTime.Now,
                    RightId = toUser.ID, // to
                    Message = (message + " " + notAllowedResponse).Trim(),
                    IsBlocked = notAllowed
                };

                //var _words = _customerService.GetAllWords().Select(x => x.Value.ToLower()).ToList();
                List<string> words = _postService.GetBadWordsCach();

                var enforceMode = _configuration["BadWordsEnforceMode"].EnumTryParse(BadWordsEnforceMode.Unspecified);
                bool hasBadWords = words.Any(word => message.ContainsBadWord(word, enforceMode));
                if (hasBadWords)
                {
                    notAllowed = true;
                    msg.IsBlocked = notAllowed;
                    message = (message + " " + _localizerMessages["BadWordsNotAllowed"]).Trim();
                }

                if (!notAllowed)
                    _messageRepository.Insert(msg);

                string sender = Context.UserIdentifier;
                User fromUser = _userRepository.Table.Include(c => c.ItemColor).FirstOrDefault(u => u.Email.ToLower() == sender.ToLower().Trim());
                MessageModelHub model = new MessageModelHub(msg, toUser, fromUser);
                model.toUserInfo = new UserInfo(toUser, _postService, _customerService, _messageService);

                if (notAllowed)
                    await Clients.User(fromUser.Email).SendAsync("ReceiveMessage", model);
                 else
                    await Clients.User(toUser.Email).SendAsync("ReceiveMessage", model);
            }
        }

        public async Task SendNewStory(int sId)
        {
            StoryModelCron model = _postService.SendStoryApproved(sId);
            if(model == null)
                return;

            foreach (var reciever in model.receivers)
            {
                    StoryModelHub result = new StoryModelHub() { 
                        storyData = model.storyData, 
                        userInfo = new UserInfo(reciever, _postService, _customerService, _messageService),
                        //fill out notification for story owner only
                        notificationData = model.storyData.UserId == reciever.ID ?
                            model.notificationData?.Select(n => new NotificationModel(n, _customerService, _postService, _localizerNotifications, _commentsRepository)).ToList() : null
                    };
                await Clients.User(reciever.Email).SendAsync("ReceiveStory", result);
            }
        }

        public async Task SendNewNotification(int id)
        {
            var UserId = Int32.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var s = _postService.GetStoryById(id);
            var notification = _notificationRepository.Table.Where(c => c.StoryId == s.ID && c.IsRead == (int)NotificationStatus.Unread);
            if (notification.Count() <= 0)
                return;

            var lastNotification = notification.Last();
            var typeList = new int[2]{(int)NotificationType.USER_LIKED,(int)NotificationType.USER_COMMENTED}; //donot send notification to self
            if ((UserId != s.UserID ) || (UserId == s.UserID && !typeList.Contains(lastNotification.Type))) // if sender and reciever are not the same
            {
                var notificationData = new NotificationModel(lastNotification, _customerService, _postService, _localizerNotifications, _commentsRepository);
                var toUser = _customerService.GetUserById(s.UserID);
                NotificationModelHub result = new NotificationModelHub() { notificationData = notificationData, userInfo = new UserInfo(toUser, _postService, _customerService, _messageService) };

                await Clients.User(toUser.Email).SendAsync("ReceiveNotification", result);
            }
        }

        //public async Task SendAdminApp()
        //{
        //    await Clients.All.SendAsync("ReceiveStory", "Test");
        //}
    }


    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value;
            // or so
            //return connection.User?.Identity.Name;
        }
    }
}
