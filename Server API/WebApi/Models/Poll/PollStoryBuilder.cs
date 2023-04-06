using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Models.Posts;
using Entities;
using Services.Posts;

namespace WebApi.Models.Poll
{
    public class PollStoryBuilder : IPollStoryBuilder
    {
        private IPostService _postService;
        private IStagingRepository<User> _usersRepository;
        private IStagingRepository<ItemColor> _colorRepository; 
        public PollStoryBuilder(
            IPostService postService, 
            IStagingRepository<User> userRepository,
            IStagingRepository<ItemColor> colorRepository
            )

        {
            _postService = postService;
            _usersRepository = userRepository;
            _colorRepository = colorRepository; 
        }

        public PollStory Build(PostPresentationModel presentation, int userId)
        {
            var story = new PollStory
            {
                Default = presentation,
                Story = presentation.Story,
                Question = presentation.Story.Title,
                Options = _postService.GetPollVariantOptions(presentation.Story.ID, userId).ToList(),
                UserAge = DateTime.Now.Year - _usersRepository.GetById(presentation.Story.UserID).YearOfBirth,
                UserNick = _usersRepository.GetById(presentation.Story.UserID).NickName,
                UserColor = _colorRepository.GetById(_usersRepository.GetById(presentation.Story.UserID).ItemColorID).ColorHex,
                UserGender = _usersRepository.GetById(presentation.Story.UserID).Gender.ToString()
        };
            return story;
        }
    }
}
