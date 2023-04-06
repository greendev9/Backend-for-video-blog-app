using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Polls
{


    public class Vote : IEntityBase
    {
        public int ID { set; get; }

        public int StoryId { set; get; }

        public int UserId { set; get; } 

        public string OptionSelected { set; get; } 
    }




}
