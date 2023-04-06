using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Localization
{
    public  interface ILocalizationService
    { 
        Task<string> DetectLangaugeAsync(string post);  


    }
}
