using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Notifications
{
    public enum CommentNotificationType
    {
        IsPending = 0,
        IsPendingApproval = 1,
        IsDeniedApproval = 2,
    }
}
