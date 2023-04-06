using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class StoryComment : IMapInterface
    {
        public int LeftId { get; set; }
        public virtual Story Story { get; set; }
        public int RightId { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
