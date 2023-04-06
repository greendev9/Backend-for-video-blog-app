using AutoMapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApi.Models.Mapping
{
    public class PostMapping : Profile
    {
        public PostMapping()
        {
            CreateMap<StoryModel, Story>()  
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember,dstMember) => 
                
                srcMember != null  || 
                ( srcMember!=null &&  int.TryParse(srcMember.ToString(), out int srInt) && srInt>0 )




                ));
            



        }
    }
}
