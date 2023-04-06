using DAL;
using Domain;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Messages
{
   
    public class MessageService : IMessageService
    {

        private readonly IStagingRepository<MessageUser> _repository; 

        public MessageService(IStagingRepository<MessageUser> repository)
        {
            _repository = repository; 
        }

        public List<int> GetUnreadMessages(int id) // should count per user not messages (3 messages from 1st user + 7 from the 2nd = 2)
        {
            var users = _repository.Table.Where(m => m.RightId == id && !m.IsRead && m.IsBlocked != true)
                .Select(o => o.LeftId).Distinct().ToList(); 
            return users;
        }

        public void ClearInbox(int from, int toId, int requestUserId)
        {
            var db = _repository.GetContext() as DataContext;
            db.Database.ExecuteSqlCommand($"update [MessageUsers] set [IsClearedRight]=1, [isRead]=1 where LeftId={from} and RightId={toId}");   //Note: delete conversation one side feature.
            db.Database.ExecuteSqlCommand($"update [MessageUsers] set [IsClearedLeft]=1 where RightId={from} and LeftId={toId}");
        }


        public void SetMessageStatus(int fromId, int toId, bool isRead) // Mark all messages from selected user as read/unread
        {
            var messages = _repository.Table.Where(u => u.LeftId == fromId && u.RightId == toId).ToList();
            messages.ForEach(x =>
            {
                if (x.IsRead != isRead)
                {
                    x.IsRead = isRead;
                    _repository.Update(x);
                }
            });
        }

        public void ClearInboxDelete(int from, int toId)   
        {
            var db = _repository.GetContext() as DataContext;
            db.Database.ExecuteSqlCommand($"delete from [MessageUsers] where LeftId={from} and RightId={toId} ");
            db.Database.ExecuteSqlCommand($"delete from [MessageUsers] where LeftId={toId} and RightId={from} ");
        }

  }
}
