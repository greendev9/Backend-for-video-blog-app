using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public  interface INotificationTransport
    { 
        void SetDestination(string dest);

        void SetSubject(string subject);

        void SetMessage(TextReader reader);

        Task<bool> Send();  
         
    } 

}
