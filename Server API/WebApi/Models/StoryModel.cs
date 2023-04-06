using Domain.Models.Posts;
using Entities;
using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class StoryModel
   {
    public int ID { get; set; }
    public StoryStatus StoryStatus { get; set; }
    public bool IsCommentsAlowed { get; set; }

    //[MaxLength(25, ErrorMessage = "Title Max length 25 chars ")]
    [StringLength(300)]
    public string Title { get; set; }
    public string Description { get; set; }
    // [MinLength(100, ErrorMessage = "Story Length must be more than 100 characters ")]
    [Required]
    public string Body { get; set; }
    public int UserID { get; set; }
    public List<int> StoryTopics { get; set; }
    public string Language { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public bool IsPending { get; set; }  //is pending approval

    public StoryModel() {}

    public StoryModel(Story s ) 
    {
            ID = s.ID;
            StoryStatus = s.StoryStatus;
            IsCommentsAlowed = s.IsCommentsAlowed;
            Title = s.Title;
            Description = s.Description;
            Body = s.Body;
            UserID = s.UserID;
            List<int> stopics = new List<int>();
            s.StoryTopic?.ForEach(x => { stopics.Add(x.RightId); });
            StoryTopics = stopics;
            Language = s.Language;
            Category = s.Category;
            SubCategory = s.SubCategory;
        }

    public StoryModel(int id, StoryStatus storyStatus, bool isCommentsAlowed, string title, string description, string body, int userID, List<int> storyTopics, string language, string category, string subCategory, bool isPending)
    {
      ID = id;
      StoryStatus = storyStatus;
      IsCommentsAlowed = isCommentsAlowed;
      Title = title;
      Description = description;
      Body = body;
      UserID = userID;
      StoryTopics = storyTopics;
      Language = language;
      Category = category;
      SubCategory = subCategory;
      IsPending = isPending;
    }
  }

    public class StoryModelHub
    {
        public PostPresentationModel storyData { get; set; }
        public UserInfo userInfo { get; set; }
        public IList<NotificationModel> notificationData { get; set; }
    }

}
