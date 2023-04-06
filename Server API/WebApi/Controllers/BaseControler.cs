using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
     
    public class BaseControler : Controller
    { 
        protected object CreateResponse(object data=null, bool success =true,
                                    IEnumerable<Error> errors = null)
        { 
            return new ApiResponse { 
                 Success = success , 
                 Payload = data , 
                 Errors =  errors  ??  new List<Error>()  
            }; 
        }
         
        protected int UserId { get {

                if (User.Identity.IsAuthenticated)
                {
                    var claim = User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
                    if (!string.IsNullOrEmpty(claim))
                        return int.Parse(claim);  
                }
                return 0;
            } }


    }
}