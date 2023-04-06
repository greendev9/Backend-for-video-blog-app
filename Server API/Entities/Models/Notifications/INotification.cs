using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Notifications
{
    public  interface INotification 
    {
         
        string GetFormattedMessage();
        string GetDestination(); 
        string GetFrom(); 
        string Getsubject();  

    }
}
