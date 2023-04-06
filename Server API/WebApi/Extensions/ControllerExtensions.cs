using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Extensions
{
    public static  class ControllerExtensions
    {

        public static IEnumerable<Error>  GetErrors(this ModelStateDictionary model)
        {  
            var result = from ms in model
                         where ms.Value.Errors.Any()
                         let fieldKey = ms.Key
                         let errors = ms.Value.Errors
                         from error in errors
                         select new Error(fieldKey, error.ErrorMessage);
             
            return result; 
        }

    }
}
