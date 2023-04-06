using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Messages
{
    public class MessageToken
    {
        private string _token;
        private readonly string _value;  
        public MessageToken(string token, string  value)
        {
            _token = "{"+token+"}";
            _value = value;  
        } 
        public override string ToString()
        => string.Format("{0}", _token);

       public string Value { get { return _value; } }



    } 
     



}
