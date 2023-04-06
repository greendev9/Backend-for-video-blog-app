using Domain.Interfaces;
using Domain.Models.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Services.Notifications
{
    public class NotificationService : INotificationService
    {  

        private INotificationTransport _defaultTransport;

        public NotificationService(INotificationTransport defaultTransport)
        {
            _defaultTransport = defaultTransport;
        } 

        public async Task<bool> SendNotificationAsync(INotification notification)
        {
            if (_defaultTransport == null)
                throw new InvalidOperationException("Transport not set");
            _defaultTransport.SetSubject(notification.Getsubject());
            var reader = new StringReader(notification.GetFormattedMessage());
            _defaultTransport.SetMessage(reader);
            _defaultTransport.SetDestination(notification.GetDestination());
            return await _defaultTransport.Send() ;
        }

        public void SetTransport(INotificationTransport transport)
        {
            _defaultTransport = transport;
        }
    }
}
