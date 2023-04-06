using Data.Model;
using Domain.Interfaces;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data.Providers
{
    public class LanguageDetectorProvider : ILangDetectorProvider
    {

        private readonly string _key;

        public LanguageDetectorProvider(string key)
        {
            _key = key;
        }

        public LanguageDetectorProvider()
        {
        }

        private bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        } 

         
         
        public async Task<string> DetectLanguageCodeAsync(string data)
        { 
            if (Regex.IsMatch(data, @"^[\p{IsHebrew}]+$") || ContainsUnicodeCharacter(data))
                return "HE";
          //var client = new RestClient("http://apilayer.net/api/detect?access_key="+_key);
            var client = new RestClient("http://api.languagelayer.com/detect?access_key=" + _key);
            client.Authenticator = new HttpBasicAuthenticator(_key, "");
            var request = new RestRequest("", Method.GET);
            request.AddParameter("query", data);
            request.AddParameter("access_key", _key);
            IRestResponse response = await client.ExecutePostTaskAsync(request);
            RestSharp.Deserializers.JsonDeserializer deserializer = new RestSharp.Deserializers.JsonDeserializer();
            var result = deserializer.Deserialize<Result>(response);
            if (result.success)
                return result.results.FirstOrDefault()?.language_code.ToUpper(); 
            return null;
        }


    }
}
