using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebApi.Utils
{

    public class EmailTransport : INotificationTransport
    {

        readonly SmtpClient client;
        string _destination;
        string _subject;
        string _message;
        string _from;
        string _fromName;

        public EmailTransport()
        {
            client = new SmtpClient();
        }

        public EmailTransport(string username,
                                string password, 
                                string from,
                                string fromName,
                                string host, int port = 587) : this()
        {
            _from = from;
            _fromName = fromName;
            System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential(username, password);
            client.Host = host;
            client.Port = port;
            client.UseDefaultCredentials = true;
            client.Credentials = myCredential;
            client.ServicePoint.MaxIdleTime = 1;
            //client.EnableSsl = false;
            client.EnableSsl = true;
        }


        public async Task<bool> Send()
        {
            bool result = false;
            try
            {
                MailMessage myMessage = new MailMessage();
                myMessage.From = new MailAddress(_from, _fromName);
                myMessage.To.Add(new MailAddress(_destination));
                myMessage.Subject = _subject;
                myMessage.IsBodyHtml = true;
                myMessage.Body = _message;
                await client.SendMailAsync(myMessage);
                result = true;
            }
            catch (Exception e)
            {
                
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!"+e.Message) ;
                result = false;
            }
            return result;
        } 
        public void SetDestination(string dest) => _destination = dest;
        public void SetMessage(TextReader reader) => _message = reader.ReadToEnd();
        public void SetSubject(string subject) => _subject = subject;

    }

}
