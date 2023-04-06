using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Customers
{
     
    public   class Report : IEntityBase
    { 
        public int ID { set; get; }

        public DateTime? ReportDate { set; get; } 

        public string Reported { set; get; }  

        public string ReportSubject { set; get; }  

        public string Comment { set; get; }  

    }
}
