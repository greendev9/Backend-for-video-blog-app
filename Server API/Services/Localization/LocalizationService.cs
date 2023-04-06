using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Localization
{ 


    public class LocalizationService 
                                : ILocalizationService
    { 
        private readonly ILangDetectorProvider _langDetector;  
        public LocalizationService(ILangDetectorProvider langDetector) 
            => _langDetector = langDetector;  
        public async Task<string> DetectLangaugeAsync(string post)=>
            await _langDetector.DetectLanguageCodeAsync(post);
         
    }
}
