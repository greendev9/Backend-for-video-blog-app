using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Extended
{
    public class AnswerExtended
    {
        public AnsweredQuestion Answer { get; set; }
        public User User { get; set; }
        public ItemColor ItemColor { get; set; }
    }
}
