using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Extended
{
    public class CommentExtended
    {
        public CommentModel Comment { get; set; }
        public UserInCommentModel User { get; set; }
        public ColorModel ItemColor { get; set; }
    }

    public class CommentModel
    {
        public int ID { get; set; }
        public DateTime PublishDate { get; set; }
        public string Body { get; set; }
        public int UserId { get; set; }
        public int StoryId { get; set; }
        public string Language { get; set; }
        public bool? IsPendingApproval { get; set; }
        public bool? IsDeniedApproval { get; set; }
        //public UserInCommentModel User { get; set; }
        //public ColorModel ItemColor { get; set; }
        public CommentModel() { }
        public CommentModel(Comment c)
        {
            ID = c.ID;
            PublishDate = c.PublishDate;
            Body = c.Body;
            UserId = c.UserId;
            StoryId = c.StoryId;
            Language = c.Language;
            IsPendingApproval = c.IsPendingApproval;
            IsDeniedApproval = c.IsDeniedApproval;
            //User = c.User != null ? new UserInCommentModel(c.User) : null;
            //ItemColor = c.User != null ? new ColorModel(c.User.ItemColor) : null;
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
            NickName = u.NickName; 
            YearOfBirth = u.YearOfBirth;
        }
    }

    public class ColorModel
    {
        public int ID { get; set; }
        public int Index { get; set; }
        public string Value { get; set; }
        public string ColorHex { get; set; }
        public string FatherName { get; set; }
        public bool IsSpecial { get; set; }
        public int? FatherId { get; set; }
        public ColorModel() { }
        public ColorModel(ItemColor c)
        {
            ID = c.ID;
            Index = c.Index;
            Value = c.Value;
            ColorHex = c.ColorHex;
            FatherName = c.FatherName;
            IsSpecial = c.IsSpecial;
            FatherId = c.FatherId;
        }

    }
}
