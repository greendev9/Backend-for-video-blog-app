using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DAL;
using Domain;
//using Domain.Models.Customers;
using Entities;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace Services.Customers
{

    public class CustomerService : ICustomerService
    {
        private readonly IStagingRepository<User> _repository;
        private readonly IStagingRepository<UserRole> _roleRepository;
        private readonly IStagingRepository<BlockedUser> _blockedRepository;
        private readonly IStagingRepository<HidenUser> _hiddenRepository;
        private readonly IStagingRepository<ReportedUsers> _reportsRepository;
        private readonly IStagingRepository<MessageUser> _messageRepository;
        private readonly IStagingRepository<TailedUsers> _tailedUsersRepository;
        private readonly IStagingRepository<TailedStoryes> _tailedStoryesRepository;
        private readonly IStagingRepository<Word> _wordRepository;
        private readonly IStagingRepository<Comment> _commentRepository;
        private readonly IStagingRepository<LikedUserStory> _likeRepository;
        private readonly IStagingRepository<Story> _storyRepository;
        private readonly IStagingRepository<Notification> _notificationsRepository;


        public CustomerService(IStagingRepository<User> repository,
            IStagingRepository<UserRole> roleRepository,
            IStagingRepository<BlockedUser> blockedRepo,
            IStagingRepository<HidenUser> hiddenRepo,
            IStagingRepository<ReportedUsers> reportRepository,
            IStagingRepository<MessageUser> messageRepository,
            IStagingRepository<TailedUsers> tailedUsers,
            IStagingRepository<TailedStoryes> tailedStoryes,
            IStagingRepository<Word> wordRepository,
            IStagingRepository<Comment> commentRepository,
            IStagingRepository<LikedUserStory> likeRepository,
            IStagingRepository<Story> storyRepository,
            IStagingRepository<Notification> notificationsRepository
            )
        {
            _repository = repository;
            _blockedRepository = blockedRepo;
            _hiddenRepository = hiddenRepo;
            _messageRepository = messageRepository;
            _reportsRepository = reportRepository;
            _tailedUsersRepository = tailedUsers;
            _tailedStoryesRepository = tailedStoryes;
            _wordRepository = wordRepository;
            _likeRepository = likeRepository;
            _storyRepository = storyRepository;
            _commentRepository = commentRepository;
            _notificationsRepository = notificationsRepository;
            _roleRepository = roleRepository;
        }

        public bool IsAdmin(int UserId)
        {
            bool result = false;
            var role = _roleRepository.Table.FirstOrDefault(t => t.LeftId == UserId && t.Role.Value == "Admin");
            if (role != null)
                result = true;
            return result;
        }

        public bool IsBlockedByAdmin(int userId)
        {
            bool idBlocked = false;
            var admins = _roleRepository.Table.Where(t => t.Role.Value == "Admin");
            var blocked = _blockedRepository.Table.Where(b => b.RightId == userId);
            blocked.ForEach(b =>
            {
                if (admins.Any(a => a.LeftId == b.LeftId))
                    idBlocked = true;
            });
            return idBlocked;
        }

        public int GetAllBlockedByAdmin()
        {
            var admins = _roleRepository.Table.Where(t => t.Role.Value == "Admin");
            var blocked = _blockedRepository.Table.Where(b => admins.Any(a => a.LeftId == b.LeftId));
            return blocked.Count();
        }

        public int GetAllUsersBySex(Entities.Enums.Gender sex)
        {
            var users = _repository.Table.Where(t => t.Gender == sex);
            return users.Count();
        }

        public IList<Word> GetAllWords()
        {
            return _wordRepository.Table.ToList();
        }

        public void Block(int id, int otherId)
        {
            Unblock(id, otherId);
            _blockedRepository.Insert(new BlockedUser { LeftId = id, RightId = otherId, BlockDate = DateTime.Now });
        }

        public void Block(int userId, int id, string reason)
        {
            Unblock(userId, id);
            _blockedRepository.Insert(new BlockedUser { LeftId = userId, RightId = id, Comment = reason, BlockDate = DateTime.Now });
        }

        public string Unblock(int whoId, int whomId)
        {
            var record = _blockedRepository.Table.FirstOrDefault(b => b.LeftId == whoId && b.RightId == whomId);
            if (record != null)
            {
                int daysLeft = 15 - (DateTime.Now - record.BlockDate).Days;
                if (daysLeft < 0)
                    _blockedRepository.Delete(record);
                else
                    return "This account can be unblocked after " + daysLeft.ToString() + " days";
            }
            else
                return "Record was not found";
            return "Success";

        }

        //This one for Cron is not used for now
        public void BlockedAlertSent(BlockedUser blocked)
        {
            var record = _blockedRepository.Table.FirstOrDefault(b => b.LeftId == blocked.LeftId && b.RightId == blocked.RightId);
            record.BlockDate = record.BlockDate.AddMinutes(-1.00);
            _blockedRepository.Update(record);
        }

        public User FindByEmail(string email)
        {
            return _repository.Table.Include(c => c.ItemColor).FirstOrDefault(u => u.Email.ToLower() == email.ToLower().Trim());
        }

        public User FindByNickName(string nick)
        {
            return _repository.Table.Include(c => c.ItemColor).FirstOrDefault(u => u.NickName.ToLower() == nick.ToLower().Trim());
        }

        public IList<BlockedUser> GetAllBlocked()
        {
            var result = new List<BlockedUser>();

            _repository.Table.Where(u => u.BlockedUser.Count > 0).ToList().ForEach(f =>
            {
                var u = GetUserById(f.ID);
                result.AddRange(u.BlockedUser);
            });
            return result;
        }

        public IList<User> GetAllUserToBlock()
        {
            var result = new List<User>();

            var blockedUsers = _blockedRepository.Table.ToList();
            _repository.Table.ToList().ForEach(f =>
            {
                f.Blocked = blockedUsers.Any(u => u.RightId == f.ID);
                result.Add(f);
            });

            //_repository.Table.Include(x => x.BlockedUser).ToList().ForEach(f =>
            //  {
            //      f.Blocked = f.BlockedUser.Any();
            //      result.Add(f);
            //  });
            return result;
        }

        public string GetUserColor(int id)
        {
            string color;

            var context = _reportsRepository.GetContext() as DataContext;
            var user = context.Users.Where(u => u.ID == id).Include(u => u.ItemColor).Include(u => u.Comments).FirstOrDefault();
            color = user.ItemColor?.ColorHex;
            return color;
        }

        public ItemColor GetUserItemColor(int id)
        {
            ItemColor itemColor;

            var context = _reportsRepository.GetContext() as DataContext;
            var user = context.Users.Where(u => u.ID == id).Include(u => u.ItemColor).FirstOrDefault();
            itemColor = user?.ItemColor;
            return itemColor;
        }

        public User GetUserByIdNotif(int id)
        {
            var context = _reportsRepository.GetContext() as DataContext;
            var user = context.Users.Where(u => u.ID == id).Include(u => u.ItemColor).Include(u => u.Comments).FirstOrDefault();
            return user;
        }

        public string GetUserEmail(int id)
        {
            var context = _reportsRepository.GetContext() as DataContext;
            //string email = context.Users.Where(u => u.ID == id).FirstOrDefault().Email;
            var user = context.Users.Where(u => u.ID == id).Include(u => u.ItemColor).Include(u => u.Comments).FirstOrDefault();

            return user.Email;
        }

        public int GetUserCounter(int id)
        {
            var context = _reportsRepository.GetContext() as DataContext;
            var user = context.Users.Where(u => u.ID == id).Include(u => u.Comments).FirstOrDefault();
            return DateTime.Now.Year - user.YearOfBirth;
        }

        public IList<HidenUser> GetAllHidden()
        {
            var result = new List<HidenUser>();

            _repository.Table.Where(u => u.HidenUser.Count > 0).ToList().ForEach(f =>
            {
                var u = GetUserById(f.ID);
                result.AddRange(u.HidenUser);
            });

            return result;
        }

        public IList<BlockedUser> GetBlockedUsers(int userId)
        {
            var user = GetUserById(userId);
            return user.BlockedUser.ToList();
        }

        public IList<HidenUser> GetHiddenUsers(int userId)
        {
            var user = GetUserById(userId);
            return user.HidenUser?.ToList();
        }

        public IList<ReportedUsers> GetReports()
        {
            return _reportsRepository.Table.OrderByDescending(r => r.ReportDate).ToList();
        }

        public User GetUserById(int userId)
        {
            var user = _repository.Table.Include(u => u.BlockedUser)
                .Include(u => u.HidenUser).Where(u => u.ID == userId).FirstOrDefault();

            user.ItemColor = GetUserItemColor(user.ID);
            //user.Email = null; user.Password = null;user.EmailConfirmToken = null;

            user.BlockedUser?.ToList().ForEach(u =>
            {
                u.OUser = _repository.Table.FirstOrDefault(ou => ou.ID == u.RightId);

            });

            user.HidenUser?.ToList().ForEach(u =>
            {
                u.OUser = _repository.Table.FirstOrDefault(ou => ou.ID == u.RightId);

            });

            return user;
        }

        public void Hide(int id, int otherId)
        {
            UnHide(id, otherId);
            _hiddenRepository.Insert(new HidenUser
            {
                LeftId = id,
                RightId = otherId
            });
        }

        public User RegisterCustomer(User user)
        {
            user.EmailConfirmToken = Guid.NewGuid().ToString();   //Note: generate email confirm token.
            user.EmailConfirmed = true;
            _repository.Insert(user);
            return _repository.GetById(user.ID);
        }

        public bool ApproveEmail(int id, string token)
        {
          User user;
          if ((user =_repository.GetById(id)).EmailConfirmToken != token)
            return false;

          user.EmailConfirmed = true;
          _repository.Update(user);
          return true;
        }

        public void SendMessage(MessageUser messageUser)
        {
            _messageRepository.Insert(messageUser);
        }

        public void SendReport(string from, string comment, string nick)
        {
            _reportsRepository.Insert(new ReportedUsers
            {
                LeftId = FindByNickName(from).ID,
                RightId = FindByNickName(nick).ID,
                Comment = comment,
                ReportDate = DateTime.Now
            });
        }

        public void Tail(int userId, int id)
        {
            if (!_tailedUsersRepository.Table.Any(t => t.LeftId == userId && t.RightId == id))
                _tailedUsersRepository.Insert(new TailedUsers
                {
                    LeftId = userId,
                    RightId = id,
                    TailedDate = DateTime.Now
                });
        }

        public void UnTail(int leftUserId, int rightUserId)
        {
            var stories = _storyRepository.Table.Where(p => p.UserID == rightUserId).Select(s => s.ID).ToList();
            if (stories.Count() > 0)
            {
                var tailedStories = _tailedStoryesRepository.Table.Where(s => s.uId == leftUserId && stories.Contains(s.sId)).ToList();
                tailedStories.ForEach(ts =>
                {
                    _tailedStoryesRepository.Delete(ts);
                });
            };
            var tailed = _tailedUsersRepository.Table.FirstOrDefault(t => t.LeftId == leftUserId && t.RightId == rightUserId);
            if (tailed != null)
                _tailedUsersRepository.Delete(tailed);
        }

        public void UnHide(int id, int otherId)
        {
            var record = _hiddenRepository.Table.FirstOrDefault(b => b.LeftId == id && b.RightId == otherId);
            if (record != null)
                _hiddenRepository.Delete(record);
        }

        public void UpdateUser(User user)
        {
            _repository.Update(user);
        }

        public bool UserExists(string email, string password) =>
            _repository.Table.Any(u => u.Email.ToLower() == email.ToLower() && u.Password == password.Trim());

        public IList<User> MyBlaBlog(int userId) // user who follows me
        {
            var TailedIds = _tailedUsersRepository.Table.Where(p => p.RightId == userId).Select(p => p.LeftId).ToList(); // user who follows me
            var HiddenIds = _hiddenRepository.Table.Where(p => p.LeftId == userId).Select(p => p.RightId).ToList();
            var BlockedIds = _blockedRepository.Table.Where(p => p.LeftId == userId).Select(p => p.RightId).ToList();
            var BlockedIds2 = _blockedRepository.Table.Where(p => p.RightId == userId).Select(p => p.LeftId).ToList();
            var result = HiddenIds.Concat(BlockedIds); //concat here to make one for condition
            result = result.ToList().Concat(BlockedIds2); //concat here to make one
            HiddenIds = result.ToList();

            var ImMyTails = _repository.Table.Include(p => p.ItemColor).Where(p => TailedIds.Contains(p.ID) && !HiddenIds.Contains(p.ID)).ToList();
            return ImMyTails;
        }

        public IList<User> ImTailing(int userId) // users im following
        {
            //var myTails = _tailedUsersRepository.Table.Join(_hiddenRepository.Table, tu => tu.LeftId, hu => hu.LeftId, (tu, hu) => new { tu, hu });
            //myTails = myTails.Where(t => !_tailedUsersRepository.Table.Any(u => u.LeftId == userId && t.tu.RightId == t.hu.RightId));

            var ImTailedIds = _tailedUsersRepository.Table.Where(p => p.LeftId == userId).Select(p => p.RightId).ToList(); // users im following
            var HiddenIds = _hiddenRepository.Table.Where(p => p.LeftId == userId).Select(p => p.RightId).ToList();
            var BlockedIds = _blockedRepository.Table.Where(p => p.LeftId == userId).Select(p => p.RightId).ToList();
            var BlockedIds2 = _blockedRepository.Table.Where(p => p.RightId == userId).Select(p => p.LeftId).ToList();
            var result = HiddenIds.Concat(BlockedIds); //concat here to make one for condition
            result = result.ToList().Concat(BlockedIds2); //concat here to make one
            HiddenIds = result.ToList();
            var MyTails = _repository.Table.Include(p => p.ItemColor).Where(p => ImTailedIds.Contains(p.ID) && !HiddenIds.Contains(p.ID)).ToList();


            //from tu in _tailedUsersRepository.Table 
            //join hu in _hiddenRepository.Table on tu.LeftId equals hu.RightId
            //where tu.RightId != hu.RightId
            //join bu1 in _blockedRepository.Table on tu.LeftId equals bu1.LeftId
            //select tu;
            return MyTails;
        }

        //public int ImTailingNewStoriesCount(int userId) // returns count of new stories from users im following (ImTailing)
        //{
        //    List<int> ImTailedIds = _tailedUsersRepository.Table.Where(p => p.LeftId == userId).Select(p => p.RightId).ToList();
        //    IQueryable<int> NewStories = _storyRepository.Table.Where(s => ImTailedIds.Contains(s.UserID) && (s.IsNewTail?? true) && s.StoryStatus == Entities.Enums.StoryStatus.Approved && s.Category == null).Select(s => s.ID);
        //    return NewStories.Count();
        //}

        public void UnTailAll(int userId)
        {
            var tailedStories = _tailedStoryesRepository.Table.Where(x => x.uId == userId).ToList();
            tailedStories.ForEach(ts =>
            {
                _tailedStoryesRepository.Delete(ts);
            });

            var BlaBlog = _tailedUsersRepository.Table.Where(x => x.LeftId == userId).ToList();
            BlaBlog.ForEach(t =>
            {
                _tailedUsersRepository.Delete(t);
            });
        }

        public void Deactivate(int id)
        {
            var user = GetUserById(id);
            user.Deactivated = true;
            _repository.Update(user);
        }

        public IList<int> GetNewCommentedStories(int id)
        {
            var comments = _commentRepository.Table.Where(x => x.Story.UserID == id &&
                x.Story.IsNew.HasValue && x.Story.IsNew.Value).Select(x => x.StoryId);
            var list = new List<int>();
            list.AddRange(comments);
            return list;
        }

        public IList<int> GetNewLikedStories(int id)
        {
            var likes = _likeRepository.Table.Where(x => x.Story.UserID == id &&
                x.Story.IsNew.HasValue && x.Story.IsNew.Value).Select(x => x.Story.ID);
            var list = new List<int>();
            list.AddRange(likes);
            return list;
        }

        //public string EncryptString(string text, string keyString)
        //{
        //    var key = Encoding.UTF8.GetBytes(keyString);

        //    using (var aesAlg = Aes.Create())
        //    {
        //        using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
        //        {
        //            using (var msEncrypt = new MemoryStream())
        //            {
        //                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //                using (var swEncrypt = new StreamWriter(csEncrypt))
        //                {
        //                    swEncrypt.Write(text);
        //                }

        //                var iv = aesAlg.IV;

        //                var decryptedContent = msEncrypt.ToArray();

        //                var result = new byte[iv.Length + decryptedContent.Length];

        //                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        //                Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

        //                return Convert.ToBase64String(result);
        //            }
        //        }
        //    }
        //}

        //public string DecryptString(string cipherText, string keyString)
        //{
        //    var fullCipher = Convert.FromBase64String(cipherText);

        //    var iv = new byte[16];
        //    var cipher = new byte[16];

        //    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        //    Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
        //    var key = Encoding.UTF8.GetBytes(keyString);

        //    using (var aesAlg = Aes.Create())
        //    {
        //        using (var decryptor = aesAlg.CreateDecryptor(key, iv))
        //        {
        //            string result;
        //            using (var msDecrypt = new MemoryStream(cipher))
        //            {
        //                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //                {
        //                    using (var srDecrypt = new StreamReader(csDecrypt))
        //                    {
        //                        result = srDecrypt.ReadToEnd();
        //                    }
        //                }
        //            }

        //            return result;
        //        }
        //    }
        //}

    }
}
