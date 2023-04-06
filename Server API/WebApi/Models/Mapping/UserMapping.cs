using AutoMapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models.Auth;

namespace WebApi.Models.Mapping
{
    public class UserMapping : Profile
    {

        public UserMapping()
        {
            CreateMap<CreateAccount, User>();     

        }  

    }
}
