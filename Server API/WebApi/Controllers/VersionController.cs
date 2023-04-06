using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
     
    [Route("api/v1/Version")]
    public class VersionController  : BaseControler
    {
              
        [HttpGet]
        public object Get()
        {
             return   CreateResponse(new { v = 1.0 });
        }
         
           
         
    }
}