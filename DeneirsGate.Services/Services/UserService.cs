using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public enum FriendStatus
    {
        Pending = 1,
        Accepted,
        Rejected,
        Blocked
    }

    public class UserService : DeneirsService
    {
        public List<UserViewModel> GetFriends(Guid userId)
        {
            var friends = new List<UserViewModel>();

            using (DBReset())
            {
                var keys = DB.FriendRequests.Where(x => x.UserSenderKey == userId && x.Status == (int)FriendStatus.Accepted).Select(x => x.UserReceiverKey).ToList();

                friends = DB.AspNetUsers.Where(x => keys.Contains(x.UserId)).Select(x => new UserViewModel
                {
                    Picture = x.Picture,
                    UserId = x.UserId,
                    Username = x.UserName
                }).ToList();
            }

            return friends;
        }
    }
}
