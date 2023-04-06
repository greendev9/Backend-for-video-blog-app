//using Domain.Models.Customers;
using Entities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Customers
{

    public interface ICustomerService
    {
        bool IsAdmin(int UserId);
        bool IsBlockedByAdmin(int userId);
        int GetAllBlockedByAdmin();
        int GetAllUsersBySex(Entities.Enums.Gender sex);
        User GetUserById(int userId);
        User GetUserByIdNotif(int userId);
        void UpdateUser(User user);
        IList<BlockedUser> GetBlockedUsers(int userId);
        IList<HidenUser> GetHiddenUsers(int userId);
        string Unblock(int iD, int id);
        void Block(int id, int otherId);
        void Hide(int id, int otherId);
        void UnHide(int id, int otherId);
        User FindByNickName(string nick);
        void SendReport(string from, string comment, string nick);
        IList<BlockedUser> GetAllBlocked();
        IList<User> GetAllUserToBlock();
        IList<HidenUser> GetAllHidden();
        IList<ReportedUsers> GetReports();
        bool UserExists(string email, string password);
        User RegisterCustomer(User user);
        User FindByEmail(string email);
        void Tail(int userId, int id);
        void UnTail(int userId, int id);
        void UnTailAll(int userId);
        void SendMessage(MessageUser messageUser);
        void Block(int userId, int id, string reason);

        string GetUserColor(int id);
        string GetUserEmail(int id);
        int GetUserCounter(int id);

        IList<User> MyBlaBlog(int userId);
        IList<User> ImTailing(int userId);
        void Deactivate(int id);
        IList<Word> GetAllWords();
        IList<int> GetNewCommentedStories(int id);
        IList<int> GetNewLikedStories(int id);

        bool ApproveEmail(int id, string token);
        void BlockedAlertSent(BlockedUser blocked);
    }

}
