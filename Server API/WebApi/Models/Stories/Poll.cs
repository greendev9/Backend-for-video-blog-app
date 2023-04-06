using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Stories
{
    public class Poll
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public string FirstChoice { get; set; }
        public string SecondChoice { get; set; }
        public string ThirdChoice { get; set; }
        public bool Yes { get; set; }
        public bool Maybe { get; set; }
        public bool NotSure { get; set; }
        public bool No { get; set; }
        public bool Noway { get; set; }  
        public int UserID { get; set; } 
        public string Language { get; set; }  
        public string Category { get; set; }
        public string SubCategory { get; set; } 
        public StoryStatus StoryStatus { get; set; }

    }
}
