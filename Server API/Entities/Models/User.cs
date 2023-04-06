using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class User : IEntityBase
    {
        public int ID { get; set; }
        [StringLength(100)]
        public string NickName { get; set; }
        public Gender Gender { get; set; }
        [StringLength(70)]
        public string Email { get; set; }
        public int YearOfBirth { get; set; }
        [StringLength(70)]
        public string Password { get; set; }
        public int CultureId { get; set; }
        public Country Country { get; set; }
        public int? CountryId { get; set; }
        public Language Language { get; set; }
        public int? LanguageId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
        public int? ItemColorID { get; set; }
        public ItemColor ItemColor { get; set; }
        public bool? EmailConfirmed { get; set; }  //Note: null for false
        public string EmailConfirmToken { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; } // Alex
        public virtual ICollection<Story> Storyes { get; set; }
        public virtual ICollection<LikedUserStory> LikedUserStory { get; set; }
        public virtual ICollection<ViewedUserStory> ViewedUserStory { get; set; }
        public virtual ICollection<BlockedUser> BlockedUser { get; set; }
        public virtual ICollection<HidenUser> HidenUser { get; set; }
        public virtual ICollection<ReportedUsers> ReportedUsers { get; set; }
        public virtual ICollection<TailedUsers> TailedUsers { get; set; }
        public virtual ICollection<TailedStoryes> TailedStoryes { get; set; }

        public bool? Deactivated { set; get; }
        [NotMapped]
        public bool Blocked { get; set; }
    }
}
