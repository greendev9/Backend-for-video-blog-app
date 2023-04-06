using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Messages
{
    public class MessageBuilder
    { 
        private readonly string _path; 
        private IList<MessageToken> _tokens;   
        
        public MessageBuilder(string templatePath)
        {
            _path = templatePath;
            _tokens = new List<MessageToken>(); 
            if (!File.Exists(templatePath))
                throw new InvalidDataException("Template file not found");   
        } 
        public void AddToken(MessageToken token) => _tokens.Add(token);   
        public async Task<string> BuildAsync()
        {
            string message =  await File.ReadAllTextAsync(_path);  
            foreach (var token in _tokens)
                message = message.Replace(token.ToString(), token.Value);  
            return message;  
        }


    }
}
