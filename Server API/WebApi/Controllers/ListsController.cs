using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Services.Localization;

namespace WebApi.Controllers
{

    [Route("/api/v1/lists")]
    [AllowAnonymous]
    public class ListsController : BaseControler
    {

        private readonly IStagingRepository<Country> _countriesRepository;
        private readonly IStagingRepository<Topic> _topicRepository;
        private readonly IStagingRepository<Language> _langRepository;  
        private readonly IStagingRepository<ItemColor> _colorsRepository ;
        private readonly IStringLocalizer<ListsController> _localizer;

        public ListsController(
            IStagingRepository<Country> countryRepo,  
            IStagingRepository<Language> langRepo ,  
            IStagingRepository<ItemColor> colorRepo, 
            IStagingRepository<Topic> topicRepo,
            IStringLocalizer<ListsController> localizer
            )
        {
            _countriesRepository = countryRepo;

            _topicRepository =  topicRepo; 
            _langRepository  = langRepo; 
            _colorsRepository =  colorRepo;
            _localizer = localizer;
        }

        /// <summary>
        /// Get List of Countries.
        /// Input: No parameters; Output: List(Country)
        /// </summary>
        /// <returns> Output: List(Country) </returns>
        [Route("Countries")]
        [HttpGet]
        public object Countries()=>
            Ok(CreateResponse(_countriesRepository.Table.ToList())) ;

        /// <summary>
        /// Get List of Languages.
        /// Input: No parameters; Output: List(Language)
        /// </summary>
        /// <returns> Output: List(Language) </returns>        
        [Route("Languages")]
        [HttpGet]
        public object Languages()=> Ok(CreateResponse(_langRepository.Table.ToList())) ;

        /// <summary>
        /// Get List of Colors.
        /// Input: No parameters; Output: List(ItemColor)
        /// </summary>
        /// <returns> Output: List(ItemColor) </returns>

        [Route("Colors")]
        [HttpGet]
        public object Colors()=> Ok (CreateResponse(_colorsRepository.Table.ToList())) ;

        /// <summary>
        /// Get List of Topics.
        /// Input: No parameters; Output: List(Topic)
        /// </summary>
        /// <returns> Output: List(Topic) </returns>
        [Route("Topics")]
        [HttpGet]
        public object Topics()
        {
            var result = _topicRepository.Table.ToList();
            result.ForEach(x =>
            { x.Value = _localizer["Topic_" + x.Value.ToLower().Replace(" ", "_").Trim()]; });
            return Ok(CreateResponse(result));
        }





    }
}