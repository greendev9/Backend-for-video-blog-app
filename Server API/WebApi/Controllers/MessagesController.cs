using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DAL;
using Domain;
using Domain.Helpers;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Services.Customers;
using Services.Messages;
using Services.Posts;
using WebApi.Models;

namespace WebApi.Controllers
{
     
    /// <summary>
    /// 
    /// </summary>
    [Route("/api/v1/inbox/")]
    [Authorize]
    public class MessagesController : BaseControler
    {
        private readonly IPostService _postService;
        private readonly IStagingRepository<MessageUser> _repository;
        private readonly IStagingRepository<User> _userRepository;
        private readonly IStagingRepository<BlockedUser> _blockedRepository;
        private readonly ICustomerService _customerService;
        private readonly IMessageService _messageService;
        private readonly IStringLocalizer<MessagesController> _localizer;
        private readonly IConfiguration _configuration;

        public MessagesController(IPostService postService, 
            IStagingRepository<MessageUser> repo, 
            ICustomerService customerService, 
            IStagingRepository<User> userRepo, 
            IStagingRepository<BlockedUser> blockedRepository,
            IMessageService messageService,
            IStringLocalizer<MessagesController> localizer,
            IConfiguration configuration
            )
        {
            _postService = postService;
            _repository = repo;
            _customerService = customerService; 
            _userRepository = userRepo;
            _blockedRepository = blockedRepository;
            _messageService = messageService;
            _localizer = localizer;
            _configuration = configuration;
        }

        /// <summary>
        /// Get User Messages.
        /// Input: No parameters; Output: [inboxCount int, UserMessages List(T)]
        /// </summary>
        /// <returns> [inboxCount int, UserMessages List(T)] </returns> 
        [HttpGet]
        public object Get()
        {
            var DB = _repository.GetContext() as DataContext;
            var messages = DB.MessageUsers.Where(m => m.LeftId == UserId && m.IsClearedLeft != true ||
                m.RightId == UserId && m.IsClearedRight != true)
                .Where(msg=>msg.IsBlocked != true).ToList();
            var result = messages.OrderByDescending(m => m.MessageDate)
                .ToLookup(m => (m.RightId != UserId ? m.RightId : m.LeftId))
                .Select(x => new
                {
                    Mine = x.FirstOrDefault()?.LeftId == UserId || false,
                    Key = x.Key,
                    IsUserBlocked = (_blockedRepository.Table.Any(b => b.RightId == x.Key && b.LeftId == UserId)),
                   // || _blockedRepository.Table.Any(b => b.LeftId == x.Key && b.RightId == UserId)),
                   // _blockedRepository.Table.Any(b => b.RightId == x.Key && b.LeftId == UserId),
                    Count = x.Count(), //Alex
                    Messages = x.OrderBy(im => im.MessageDate).ToList(),
                    LeftUser = x.FirstOrDefault()?.LeftId > 0 ? _userRepository.Table.Where(u => u.ID == x.FirstOrDefault().LeftId).Select( i=> new { i.ID, i.NickName, i.Gender, i.ItemColor, i.Language, i.YearOfBirth })//DB.Users.Include(u => u.ItemColor)
                                .FirstOrDefault(um => um.ID == x.FirstOrDefault().LeftId) : null,
                    RightUser = x.FirstOrDefault()?.LeftId > 0 ? _userRepository.Table.Where(u => u.ID == x.FirstOrDefault().RightId).Select(i => new { i.ID, i.NickName, i.Gender, i.ItemColor, i.Language, i.YearOfBirth })// DB.Users.Include(u => u.ItemColor)
                                .FirstOrDefault(um => um.ID == x.FirstOrDefault().RightId) : null
                }).ToList();


            return CreateResponse(data: new { inboxCount = _messageService.GetUnreadMessages(UserId).Count(), UserMessages = result });
        }

        /// <summary>
        /// Post New Message.
        /// Input: CreateMessage Object; Output: success = true/false
        /// </summary>
        /// <typeparam name = "model"> CreateMessage </typeparam>
        /// <returns> success = true/false </returns>
        [HttpPost]
        public object Post([FromBody] CreateMessage model)
        {
            var user = _customerService.FindByNickName(model.nick);

            var blockedUsers = _userRepository.TableNoTracking
                    .Include(p => p.HidenUser).Include(d => d.BlockedUser).First(p => p.ID == user.ID); 

            if (blockedUsers.HidenUser.Any(h => h.RightId == UserId && h.LeftId == user.ID))
                return CreateResponse(null, false, new List<Error> { new Error("id", _localizer["HidenUser"]) });
            else if (blockedUsers.BlockedUser.Any(h => h.RightId == UserId && h.LeftId == user.ID))
                return CreateResponse(null, false, new List<Error> { new Error("id", _localizer["YouBlockedByUser"]) });

            blockedUsers = _userRepository.TableNoTracking
                                .Include(p => p.HidenUser).Include(d => d.BlockedUser).First(p => p.ID == UserId);

            if (blockedUsers.BlockedUser.Any(h => h.RightId == user.ID && h.LeftId == UserId))
                return CreateResponse(null, false, new List<Error> { new Error("id", _localizer["YouBlockedThisUser"]) });
            //var words = _customerService.GetAllWords().Select(x => x.Value.ToLower()).ToList();
            List<string> words = _postService.GetBadWordsCach();

            MessageUser message = new MessageUser
            {
                IsRead = false,
                LeftId = UserId, // from
                MessageDate = DateTime.Now,
                RightId = user.ID, // to
                Message = model.message,
                IsBlocked = false
            };
            var enforceMode = _configuration["BadWordsEnforceMode"].EnumTryParse(BadWordsEnforceMode.Unspecified);
            bool hasBadWords = words.Any(word => model.message.ContainsBadWord(word, enforceMode));
            if (hasBadWords)     //Note: Auto approval feature.
            {
                message.IsBlocked = true;
                //model.IsPending = true;
            }

            _repository.Insert(message);
            return Ok(CreateResponse(_localizer["MessageSent"].Value, success: true));
        }

        /// <summary>
        /// Delete Selected User Messages (Clear Inbox).
        /// Input: userId; Output: inboxCount int
        /// </summary>
        /// <typeparam name = "userId"> int </typeparam>
        /// <returns> inboxCount int </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [HttpDelete]
        public object Delete([FromQuery]int userId)
        {
            try
            {
                _messageService.ClearInbox(userId, UserId, requestUserId: UserId);
                //return CreateResponse();
                return Ok(CreateResponse(data: _messageService.GetUnreadMessages(UserId).Count(), success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(success: false, data: null, errors: new Error[1] { new Error("ApplicationError", ex.Message) }));
            }
        }

        /// <summary>
        /// Change Message Status.
        /// Input: id (User ID), isRead [true, false]; Output: inboxCount int
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> inboxCount int </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Route("{id}/SetMessageStatus")]
        [HttpPut]
        public object SetMessageStatus(int id, bool isRead = true)
        {
            try
            {
                _messageService.SetMessageStatus(id, UserId, isRead);
                //return CreateResponse();
                var data = _messageService.GetUnreadMessages(UserId);
                return Ok(CreateResponse(data: new { inboxCount = data.Count(), unreadUsers = data }, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(success: false, data: null, errors: new Error[1] { new Error("ApplicationError", ex.Message) }));
            }
        }


    }
}