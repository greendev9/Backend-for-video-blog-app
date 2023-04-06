using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class MessageModel
    { 
        public string Message { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }

    public class MessageModelHub
    {

        public int toUserId { get; set; }
        public string toNickName { get; set; }
        public int toAge { get; set; }
        public string toColor { get; set; }
        public string toGender { get; set; }

        public int fromUserId { get; set; }
        public string fromNickName { get; set; }
        public int fromAge { get; set; }
        public string fromColor { get; set; }
        public string fromGender { get; set; }

        public MessageUserModel messageList { get; set; }
        public string message { get; set; }

        public UserInfo toUserInfo { get; set; }
        
        public MessageModelHub() { }
        public MessageModelHub(MessageUser mu, User toUser, User fromUser)
        {
            toUserId = toUser.ID;
            toNickName = toUser.NickName;
            toAge = DateTime.Now.Year - toUser.YearOfBirth;
            toColor = toUser.ItemColor.ColorHex;
            toGender = toUser.Gender.ToString();

            fromUserId = fromUser.ID;
            fromNickName = fromUser.NickName;
            fromAge = DateTime.Now.Year - fromUser.YearOfBirth;
            fromColor = fromUser.ItemColor.ColorHex;
            fromGender = fromUser.Gender.ToString();

            messageList = new MessageUserModel(mu);
            message = "message";
        }
    }

    public class MessageUserModel
    {
        public int ID { get; set; }
        public int LeftId { get; set; }
        public int RightId { get; set; }
        public string Message { get; set; }
        public string MessageDate { get; set; }
        public bool IsRead { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? IsClearedRight { get; set; }
        public bool? IsClearedLeft { get; set; }
        public User OUser { set; get; }
        
        public MessageUserModel() { }
        public MessageUserModel(MessageUser mu)
        {
            ID = mu.ID;
            LeftId = mu.LeftId;
            RightId = mu.RightId;
            Message = mu.Message;
            MessageDate = mu.MessageDate.ToString("dd/MM/yyyy");
            IsRead = mu.IsRead;
            IsBlocked = mu.IsBlocked;
            IsClearedRight = mu.IsClearedRight;
            IsClearedLeft = mu.IsClearedLeft;
            OUser = mu.OUser;
        }
    }

}
