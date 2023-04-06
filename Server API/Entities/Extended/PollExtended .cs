using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Extended
{
    public class PollExtended
    {
        public Poll Poll { get; set; }
        public IEnumerable<AnswerExtended> Answers { get; set; }
        public int TotalAnswers { get; set; }
        public bool LikedByMe { get; set; }
        public bool ViewedByMe { get; set; }

        public int TotalYes { get; set; }
        public int TotalMaybe { get; set; }
        public int TotalNotSure { get; set; }
        public int TotalNo { get; set; }
        public int TotalNoway { get; set; }
    }
}
