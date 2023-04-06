using Domain.Models.Polls;
using Domain.Models.Posts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Poll
{
    public class PollStory
    {

        public PostPresentationModel Default { set; get; }

        public PostModel Story { set; get; }

        public bool TailedByMe { set; get; }


        public string Question { set; get; }
        public List<PollVariantOption> Options { set; get; }
        public object UserColor { get; internal set; }
        public string UserNick { get; internal set; }
        public int UserAge { get; internal set; }
        public string UserGender { get; internal set; }
    }


}
