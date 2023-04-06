using Domain.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Poll
{
    public interface IPollStoryBuilder
    {

        PollStory Build(PostPresentationModel presentation, int userId);  


    }
}
