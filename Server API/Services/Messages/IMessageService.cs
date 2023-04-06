using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Messages
{
    public interface IMessageService
    {


        void ClearInbox(int from, int toId,int requestUserId);
        void ClearInboxDelete(int from, int toId);   //Note: Not used. delete conversation physically.
        List<int> GetUnreadMessages(int id);
        void SetMessageStatus(int userId, int id, bool isRead);
    }
}
