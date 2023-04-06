using Cultures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Enums
{
    public enum StoryStatus
    {
        None = -1 , 
        PendingApproval = 0,
        Approved = 1,
        Draft = 2,
        NotApproved = 3
    }

    public enum NotificationType : int
    {
        USER_COMMENTED = 0,
        STORY_APPROVED = 1,
        STORY_REJECTED = 2,
        STORY_PENDING = 3,
        USER_LIKED = 4
    }

    public enum NotificationStatus : int
    {
        Unread = 0,
        Read = 1,
        Clicked = 2
    }

    public enum BlockedRule : int
    {
        ForPosts = 0,
        ForUsers = 1,
        ForMessages = 2
    }
    public enum ApplicationType : int
    {
        Mobile = 0,
        Web = 1
    }

    public enum TailedStories
    {
        NewStories = 1       
    }

    public enum BuiltInAnswer
    {
        Yes = 0,
        Maybe = 1,
        NotSure = 2,
        No = 3,
        NoWay = 4
    }
    public enum Gender
    {
        //NotSet = 0,
        Male = 1,
        Female = 2
    }
    public static class EnumDicts
    {
        public static Dictionary<Gender, Dictionary<Culture, string>> GenderDict = new Dictionary<Gender, Dictionary<Culture, string>>
        {
            {Gender.Male,new Dictionary<Culture, string>{
                { CD.Cultures["he"],"זכר"},
                { CD.Cultures["ru"],"Мужчина"},
                { CD.Cultures["en"],"Male"}
            }},
            {Gender.Female,new Dictionary<Culture, string>{
                { CD.Cultures["he"],"נקבה"},
                { CD.Cultures["ru"],"женшина"},
                { CD.Cultures["en"],"Female"}
            }},
        };
    }
    public enum Category
    {
        Motherhood = 0,
        Travel = 1,
        Startup = 2,
        Food = 3
    }
    public enum SubCategory
    {
        PreBirth = 0,
        BabysFirstYear = 1,
        General = 2,
        JustArrived = 3,
        PlacesTips=4,
        Vegans=5,
        Vegetarians = 6
    }

    public enum BadWordsEnforceMode
    {
      Unspecified=0,
      Lenient=1,
      Normal=2,
      Strict=3
    }

    public static class CategoryList
    {
        public static List<Category> list = new List<Category> {
            Category.Motherhood,
            Category.Travel,
            Category.Startup,
            Category.Food
        };
        public static Dictionary<Category, List<SubCategory>> CatList = new Dictionary<Category, List<SubCategory>>
        {
            {
                Category.Motherhood,new List<SubCategory>{
                    { SubCategory.PreBirth },
                    { SubCategory.BabysFirstYear },
                    { SubCategory.General }
                }
            },
            {
                Category.Travel,new List<SubCategory>{
                    { SubCategory.JustArrived },
                    { SubCategory.PlacesTips }
                }
            },
            {
                Category.Startup,new List<SubCategory>{
                    
                }
            },
            {
                Category.Food,new List<SubCategory>{
                    { SubCategory.Vegans },
                    { SubCategory.Vegetarians },
                    { SubCategory.General }
                }
            }
        };
    }
}
