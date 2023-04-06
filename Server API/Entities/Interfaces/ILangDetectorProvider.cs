using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public  interface ILangDetectorProvider
    { 
        Task<string> DetectLanguageCodeAsync(string data);
         
    }
}
