using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Polls
{  


    public class PollVariantOption
    {   
        public string VariantName { set; get; }

        public  int Percentage { set; get; }  

        public bool Voted { set; get; }

    }    


}
