using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Entities; 
using Services.Customers;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("/api/v1/tails")]
    public class BlaBlogController : BaseControler
    {
        private ICustomerService _customerService;

        public BlaBlogController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
       
         
      
    }
     

}