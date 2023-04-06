using Domain.Interfaces;
using Domain.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Notifications
{ 
    
    public  interface INotificationService
    { 
        Task<bool> SendNotificationAsync(INotification notification);
        void SetTransport(INotificationTransport transport);
    }
}
