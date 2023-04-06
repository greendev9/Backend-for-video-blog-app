using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Models.Posts
{ 

    public class PollModel
    {


        public PollModel()
        {
            PollOptions = new List<string>();
            Voted = new Dictionary<string, int>(); 
        }
        public string FirstChoice { get; set; }
        public string SecondChoice { get; set; }
        public string ThirdChoice { get; set; }
        public string Language { set; get; }


        [Required]
        public string Question { set; get; }
        public List<string> PollOptions { set; get; } 
        public int StoryId { set; get; } 

        public Dictionary<string, int> Voted { set; get; }


        public string Category { set; get; }

        public string SubCategory { set; get; }
        public StoryStatus StoryStatus { set; get; }

        public string NickName { set; get; }

        public string ColorHex { set; get; } 

        public bool Yes { get; set; }
        public bool Maybe { get; set; }
        public bool NotSure { get; set; }
        public bool No { get; set; }
        public bool Noway { get; set; }





    }

}
