using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Model
{
    public class Detection
    {
        public string language_code { get; set; }
     
    }

  

    public class Result
    {
        public bool success { set; get; }
        public IEnumerable<Detection>  results { set; get; }
    }
}
