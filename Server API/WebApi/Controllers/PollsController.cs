using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Localization;
using Services.Posts;
using WebApi.Models;
using WebApi.Models.Poll;
using WebApi.Models.Stories;

namespace WebApi.Controllers
{


    [Route("/api/v1/polls/")]
    [Authorize]
    public class PollsController : BaseControler
    {
        private readonly IPostService _postService;
        private readonly ILocalizationService _localizationService;
        private readonly IPollStoryBuilder _pollBuilder; 

        public PollsController(
            IPostService postService,
            ILocalizationService localService,
            IPollStoryBuilder pollStoryBuilder 
            )
        {
            _postService = postService;
            _localizationService = localService;
            _pollBuilder = pollStoryBuilder;

        }

        [HttpGet]
          public object Get(        string catalog, 
                                    string subcatalog=null,
                                    string search = null, 
                                    int page = 0, int limit = 100)
        {
            var searchedpost = _postService.SearchStories(
            Title: search,
            UserId: UserId,
            includeUser:false , 
            Catalog: catalog,
            OnlyPolls: true,
            SubCatalog: subcatalog , 
            
            pageSize: limit, skip: limit * page);
            return CreateResponse(searchedpost.Select(x => _pollBuilder.Build(x, UserId)).ToList());
        }
         
        //  Create Poll model 
        [HttpPost]
        public async Task<object> Post([FromBody]Poll model)
        {
            List<string> options = new List<string>();
            if (!string.IsNullOrEmpty(model.FirstChoice))
                options.Add(model.FirstChoice);
            if (!string.IsNullOrEmpty(model.SecondChoice))
                options.Add(model.SecondChoice);
            if (!string.IsNullOrEmpty(model.ThirdChoice))
                options.Add(model.ThirdChoice);

            int queryMaxLength = 900; // http://api.languagelayer.com service restriction on query length (1000 symbols max)
            string query = model.Question.Length > queryMaxLength ? model.Question.Substring(0, queryMaxLength) : model.Question;
            if (!String.IsNullOrEmpty(query))
            {
                try { model.Language = await _localizationService.DetectLangaugeAsync(query.Trim()); }
                catch (Exception ex)
                {
                    return Ok(CreateResponse(model, false, new Error[1] { new Error("LangDetectorError", "LangDetectorError" + " " + ex.Message) }));
                }
            }
            else
                model.Language = "EN";
            if (String.IsNullOrEmpty(model.Language))
                return Ok(CreateResponse(model, false, new Error[1] { new Error("LangDetectorError", "LangDetectorError") }));
                
            //model.Language = await _localizationService.DetectLangaugeAsync(model.Question.Substring(0, 900));
            _postService.InsertOrdUpdatePoll(new Domain.Models.Posts.PollModel
            {
                FirstChoice = model.FirstChoice,
                SecondChoice = model.SecondChoice,
                ThirdChoice = model.ThirdChoice,
                StoryId = model.ID,
                Question = model.Question,
                Category = model.Category,
                SubCategory = model.SubCategory,
                PollOptions = options,
                Yes = model.Yes,
                Maybe = model.Maybe, 
                No = model.No,
                NotSure = model.NotSure,
                Noway = model.Noway,
                Language = model.Language
            }, UserId); 
            return CreateResponse(model);
        }



        [HttpPost]
        [Route("Vote")] 
        public object  Vote([FromBody] VoteRequest model)
        {  
           _postService.Vote(model.storyId, model.option, UserId);  
            return Ok(); 
        }



        [HttpPut]
        public async Task<object> Put([FromBody]Poll model)
        {

               List<string> options = new List<string>();
            if (!string.IsNullOrEmpty(model.FirstChoice))
                options.Add(model.FirstChoice);
            if (!string.IsNullOrEmpty(model.SecondChoice))
                options.Add(model.SecondChoice);
            if (!string.IsNullOrEmpty(model.ThirdChoice))
                options.Add(model.ThirdChoice);
            model.Language = await _localizationService.DetectLangaugeAsync(model.Question.Substring(0, 900));
            _postService.InsertOrdUpdatePoll(new Domain.Models.Posts.PollModel
            {
                FirstChoice = model.FirstChoice,
                SecondChoice = model.SecondChoice,
                ThirdChoice = model.ThirdChoice,
                StoryId = model.ID,
                Question = model.Question,
                Category = model.Category,
                SubCategory = model.SubCategory,
                PollOptions = options,
                Yes = model.Yes,
                Maybe = model.Maybe,
                No = model.No,
                NotSure = model.NotSure,
                Noway = model.Noway,
                Language = model.Language
            }, UserId); 

            return Ok();
        }
    }
}