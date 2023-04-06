using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ApiResponse
    {

        public bool Success { set; get; }

        public object Payload { set; get; }

        public IEnumerable<Error>  Errors { set; get; }

        //public ApiResponse()
        //{
          
        //}

        //public ApiResponse(bool success, object payload, IEnumerable<Error> errors)
        //{
        //  Success = success;
        //  Payload = payload;
        //  Errors = errors;
        //}
    }
}
