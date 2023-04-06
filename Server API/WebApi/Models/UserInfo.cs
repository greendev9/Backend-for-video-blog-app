using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Entities;
using Entities.Enums;
using Microsoft.Extensions.Localization;
using Services.Customers;
using Services.Messages;
using Services.Posts;
using WebApi.Controllers;
using Microsoft.Extensions.Configuration;
using Domain.Models.Posts;

namespace WebApi.Models
{
    public class UserInfo
    {
        public int TotalPending { set; get; }
        public int TotalDrafts { set; get; }
        public int TotalApproval { set; get; }
        public int TotalImTailing { set; get; }
        public int DeniedPublished { set; get; }
        public string Name { get; internal set; }
        public string Color { get; internal set; }
        public object Count { get; internal set; }
        public int ID { get; internal set; }
        public string Gender { get; internal set; }
        public int inboxCount { set; get; }
        public int newImTailingCount { set; get; }
        public int unread_Notifs_No { set; get; }
        public int yearOfBirth { get; set; }

        public UserInfo() { }
        public UserInfo(User user, IPostService _postService, ICustomerService _customerService, IMessageService _messageService)

        {
            UserTabStatModel tabStat = _postService.GetUserTabStat(user.ID);
            DeniedPublished = tabStat.DeniedPublished;
            TotalApproval = tabStat.TotalApproval;
            TotalDrafts = tabStat.TotalDrafts;
            TotalPending = tabStat.TotalPending;

            TotalImTailing = _customerService.ImTailing(user.ID).Count();
            newImTailingCount = _postService.NewImTailingCount(user.ID);
            
            Color = user.ItemColor != null ? user.ItemColor.ColorHex : _customerService.GetUserColor(user.ID);
            Count = DateTime.Now.Year - user.YearOfBirth; // _customerService.GetUserCounter(user.ID);

            Name = user.NickName;
            inboxCount = _messageService.GetUnreadMessages(user.ID).Count();
            ID = user.ID;
            Gender = user.Gender.ToString(); // 1male - 2 
            unread_Notifs_No = _postService.GetNewNotifications(user.ID, false).Count();
            yearOfBirth = user.YearOfBirth;
        }
    }

    public class UserModel
    {
        public int ID { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
        public int YearOfBirth { get; set; }
        public int CultureId { get; set; }
        public Country Country { get; set; }
        public int? CountryId { get; set; }
        public Language Language { get; set; }
        public int? LanguageId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
        public int? ItemColorID { get; set; }
        public string ItemColor { get; set; }
        public bool? EmailConfirmed { get; set; }  //Note: null for false
        public bool? Deactivated { set; get; }
        public bool Blocked { get; set; }
        public UserModel() { }
        public UserModel(User user)
        {
            ID = user.ID;
            NickName = user.NickName;
            Gender = user.Gender.ToString();
            YearOfBirth = user.YearOfBirth;
            CultureId = user.CultureId;
            Country = user.Country;
            CountryId = user.CountryId;
            Language = user.Language;
            LanguageId = user.LanguageId;
            CreationDate = user.CreationDate;
            LastLoginDate = user.LastLoginDate;
            UserRole = user.UserRole;
            ItemColorID = user.ItemColorID;
            ItemColor = user.ItemColor.ColorHex;
            EmailConfirmed = user.EmailConfirmed;
            Deactivated = user.Deactivated;
            Blocked = user.Blocked;
        }
    }

    public class UserInCommentModel
    {
        public int ID { get; set; }
        public string NickName { get; set; }
        public int Gender { get; set; }
        public int YearOfBirth { get; set; }
        public string Email { get; set; }
        public UserInCommentModel() { }
        public UserInCommentModel(User u)
        {
            ID = u.ID;
            Email = u.Email;
            Gender = (int)u.Gender;
            NickName = u.NickName; YearOfBirth = u.YearOfBirth;
        }
    }

    public class BlockedUserModel
    {
        public string Name { get; internal set; }
        public string Color { get; internal set; }
        public int ID { get; internal set; }
        public string Comment { get; set; }
        public string Gender { get; internal set; }
        public DateTime BlockDate { set; get; }
        public string WaitComment { get; set; }
        public int yearOfBirth { get; set; }

        public BlockedUserModel() { }

        public BlockedUserModel(BlockedUser blockedUser, ICustomerService _customerService, IStringLocalizer<ClientsController> _localizer, IConfiguration _configuration)
        {
            var user = _customerService.GetUserById(blockedUser.RightId);
            Name = user.NickName;
            Color = _customerService.GetUserColor(user.ID);
            ID = user.ID;
            Comment = blockedUser.Comment;
            Gender = user.Gender.ToString(); // 1male - 2 
            BlockDate = blockedUser.BlockDate;
            int daysLeft = (DateTime.Now - BlockDate).Days;
            int blockedDays = 0;
            Int32.TryParse(_configuration["DaysBlocked:ByUser"], out blockedDays);
            WaitComment = daysLeft < blockedDays ? _localizer["WaitUnblocking"] + " " + (blockedDays - daysLeft).ToString() : ""; //blockedDays.ToString() + " " + _localizer["LockoutExpired"]; // Per Mahesh request
            yearOfBirth = user.YearOfBirth;
        }
    }

    public class HiddenUserModel
    {
        public string NickName { get; internal set; }
        public string Color { get; internal set; }
        public int ID { get; internal set; }
        public string Gender { get; internal set; }
        public int yearOfBirth { get; set; }

        public HiddenUserModel() { }

        public HiddenUserModel(User oUser, ICustomerService _customerService)
        {
            if (oUser != null)
            {
                NickName = oUser.NickName;
                Color = _customerService.GetUserColor(oUser.ID);
                ID = oUser.ID;
                Gender = oUser?.Gender.ToString(); // 1male - 2 
                yearOfBirth = oUser.YearOfBirth;
            }
        }
    }
}
