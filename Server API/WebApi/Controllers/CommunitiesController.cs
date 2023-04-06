using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Posts;

namespace WebApi.Controllers
{

    [Route("/api/v1/community/")]
    [Authorize]
    public class CommunitiesController : BaseControler
    {

        private readonly IPostService _postService;

        public CommunitiesController(IPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        ///   
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object Get()
        {
            return CreateResponse(new
            {
                Startup = new List<string> { },
                Travel = new List<string> { "JustArrrived", "Places Tips" },
                Food = new List<string> { "Vegans", "Vegetarians", "General" },
                Motherhood = new List<string> { "PreBirth", "BabysFirstYear", "General" }
            });
        }
    }
}