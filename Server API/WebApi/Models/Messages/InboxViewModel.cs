using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Messages
{
    public class InboxViewModel
    {

        public List<MessageUser> Messages { get; set; }
    }
}
