using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    /// <summary>
    /// Ignore null From source object  
    /// </summary>
    class IgnoreNullResolver : IMemberValueResolver<object, object, object, object>
    {
        public object Resolve(object source, object destination, object sourceMember, object destinationMember, ResolutionContext context)
        {
            return sourceMember != null ? sourceMember :  destinationMember;
        }
    }
}
