using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class StoryTopic : IMapInterface
    {
        public int LeftId { get; set; }
        public virtual Story Story { get; set; }
        public int RightId { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
