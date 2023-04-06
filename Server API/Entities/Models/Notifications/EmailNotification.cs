using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Notifications
{
    public class EmailNotification : INotification 
    {
        private readonly string _from;
        private readonly string _to; 
        private readonly string _subject; 
        private readonly string _message;

        public EmailNotification(string from,  string to ,  string  subject, string message)
        {
            _from = from;
            _to = to;
            _subject = subject;
            _message = message;  
        } 
        public string GetDestination() => _to;

        public string GetFormattedMessage() => _message;

        public string GetFrom() => _from;

        public string Getsubject() => _subject;
    }
}
