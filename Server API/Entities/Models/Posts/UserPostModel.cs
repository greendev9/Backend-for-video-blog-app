using Entities;
using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models.Posts
{
    public class UserPostModel
    {
        public int ID { get; set; }
        public string NickName { get; set; }
        public Gender Gender { get; set; }
        public int YearOfBirth { get; set; }
        public ItemColorModel ItemColor { get; set; }

        public UserPostModel() { }

        public UserPostModel(User u) 
        {
            ID = u.ID;
            NickName = u.NickName;
            Gender = u.Gender;
            YearOfBirth = u.YearOfBirth;
            ItemColor = u.ItemColor != null ? new ItemColorModel { 
                ID = u.ItemColor.ID, 
                ColorHex = u.ItemColor.ColorHex, 
                Index = u.ItemColor.Index, 
                FatherName = u.ItemColor.FatherName, 
                IsSpecial = u.ItemColor.IsSpecial, 
                FatherId = u.ItemColor.FatherId
            } : null;
        }
    }

    public class ItemColorModel
    {
        public int ID { get; set; }
        public int Index { get; set; }
        public string Value { get; set; }
        public string ColorHex { get; set; }
        public string FatherName { get; set; }
        public bool IsSpecial { get; set; }
        public int? FatherId { get; set; }
    }


    public class UserTabStatModel
    {
        public int DeniedPublished { get; set; }
        public int TotalApproval { get; set; }
        public int TotalDrafts { get; set; }
        public int TotalPending { get; set; }
    }
}
