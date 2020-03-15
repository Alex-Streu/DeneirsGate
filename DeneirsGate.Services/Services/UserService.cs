using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public enum FriendStatus
    {
        None = 0,
        Pending,
        Accepted,
        Blocked
    }

    public class UserService : DeneirsService
    {
        public FriendStatus CheckFriendStatus(Guid userId, Guid checkId)
        {
            var status = FriendStatus.None;

            using (DBReset())
            {
                var check = DB.FriendRequests.FirstOrDefault(x => (x.UserSenderKey == userId && x.UserReceiverKey == checkId)
                                                                    || (x.UserSenderKey == checkId && x.UserReceiverKey == userId))?.Status;
                status = (FriendStatus)(check ?? (int)FriendStatus.None);
            }

            return status;
        }

        public List<FriendViewModel> GetFriends(Guid userId, Dictionary<string, DateTime> onlineUsers, bool onlineOnly = false)
        {
            if (onlineUsers == null) { onlineUsers = new Dictionary<string, DateTime>(); }
            var friends = new List<FriendViewModel>();

            using (DBReset())
            {
                var keys = DB.FriendRequests.Where(x => x.UserSenderKey == userId && x.Status == (int)FriendStatus.Accepted).Select(x => x.UserReceiverKey).ToList();
                keys.AddRange(DB.FriendRequests.Where(x => x.UserReceiverKey == userId && x.Status == (int)FriendStatus.Accepted).Select(x => x.UserSenderKey).ToList());

                friends = DB.AspNetUsers.Where(x => keys.Contains(x.UserId)).Select(x => new FriendViewModel
                {
                    Picture = x.Picture,
                    UserId = x.UserId,
                    Username = x.UserName,
                    IsOnline = false,
                    Status = FriendStatus.Accepted
                }).ToList();

                foreach (var friend in friends)
                {
                    friend.IsOnline = onlineUsers.ContainsKey(friend.Username);
                }

                if (onlineOnly) { friends.RemoveAll(x => !x.IsOnline); }

                friends = friends.OrderByDescending(x => x.IsOnline).ThenBy(x => x.Username).ToList();
            }

            return friends;
        }

        public List<FriendViewModel> GetRequests(Guid userId)
        {
            var friends = new List<FriendViewModel>();

            using (DBReset())
            {
                var keys = DB.FriendRequests.Where(x => x.UserReceiverKey == userId && x.Status == (int)FriendStatus.Pending).Select(x => x.UserSenderKey).ToList();
                friends = DB.AspNetUsers.Where(x => keys.Contains(x.UserId)).Select(x => new FriendViewModel
                {
                    Picture = x.Picture,
                    UserId = x.UserId,
                    Username = x.UserName,
                    Status = FriendStatus.Pending,
                    LastUpdated = DB.FriendRequests.FirstOrDefault(y => y.UserReceiverKey == userId && y.UserSenderKey == x.UserId).LastUpdated
                }).OrderBy(x => x.LastUpdated).ToList();
            }

            return friends;
        }

        public List<FriendViewModel> GetPending(Guid userId)
        {
            var friends = new List<FriendViewModel>();

            using (DBReset())
            {
                var keys = DB.FriendRequests.Where(x => x.UserSenderKey == userId && x.Status == (int)FriendStatus.Pending).Select(x => x.UserReceiverKey).ToList();
                friends = DB.AspNetUsers.Where(x => keys.Contains(x.UserId)).Select(x => new FriendViewModel
                {
                    Picture = x.Picture,
                    UserId = x.UserId,
                    Username = x.UserName,
                    Status = FriendStatus.Pending,
                    LastUpdated = DB.FriendRequests.FirstOrDefault(y => y.UserSenderKey == userId && y.UserReceiverKey == x.UserId).LastUpdated
                }).OrderBy(x => x.LastUpdated).ToList();
            }

            return friends;
        }

        public List<FriendViewModel> GetBlocked(Guid userId)
        {
            var friends = new List<FriendViewModel>();

            using (DBReset())
            {
                var keys = DB.FriendBlocks.Where(x => x.UserKey == userId).Select(x => x.BlockedUserKey).ToList();
                friends = DB.AspNetUsers.Where(x => keys.Contains(x.UserId)).Select(x => new FriendViewModel
                {
                    Picture = x.Picture,
                    UserId = x.UserId,
                    Username = x.UserName,
                    IsOnline = false,
                    Status = FriendStatus.Blocked
                }).OrderBy(x => x.Username).ToList();
            }

            return friends;
        }

        public AllFriendsViewModel GetAllFriends(Guid userId, Dictionary<string, DateTime> onlineUsers)
        {
            if (onlineUsers == null) { onlineUsers = new Dictionary<string, DateTime>(); }
            var friends = new AllFriendsViewModel();

            friends.Friends = GetFriends(userId, onlineUsers);
            friends.Requests = GetRequests(userId);
            friends.Pending = GetPending(userId);
            friends.Blocked = GetBlocked(userId);

            return friends;
        }

        public void SendFriendRequest(Guid userId, FriendRequestPostModel model)
        {
            using (DBReset())
            {
                var request = DB.FriendRequests.FirstOrDefault(x => (x.UserSenderKey == userId && x.UserReceiverKey == model.RequestUserId)
                                                                || (x.UserSenderKey == model.RequestUserId && x.UserReceiverKey == userId));
                if (request == null)
                {
                    DB.FriendRequests.Add(new FriendRequest
                    {
                        LastUpdated = DateTime.UtcNow,
                        Status = (int)FriendStatus.Pending,
                        UserReceiverKey = model.RequestUserId,
                        UserSenderKey = userId
                    });
                }
                else
                {
                    request.LastUpdated = DateTime.UtcNow;
                    request.Status = (int)FriendStatus.Pending;
                }

                DB.SaveChanges();
            }
        }

        public void UpdateFriendRequest(Guid userId, Guid senderKey, FriendStatus status)
        {
            FriendRequest request = null;

            using (DBReset())
            {
                switch (status)
                {
                    case FriendStatus.Accepted:
                    case FriendStatus.Blocked:
                        request = DB.FriendRequests.FirstOrDefault(x => (x.UserSenderKey == senderKey && x.UserReceiverKey == userId)
                                                                        || (x.UserSenderKey == userId && x.UserReceiverKey == senderKey));
                        if (request != null)
                        {
                            request.Status = (int)status;
                            request.LastUpdated = DateTime.UtcNow;
                        }
                        if (status == FriendStatus.Blocked)
                        {
                            DB.FriendBlocks.Add(new FriendBlock
                            {
                                UserKey = userId,
                                BlockedUserKey = senderKey
                            });
                        }
                        else
                        {
                            var blocked = DB.FriendBlocks.FirstOrDefault(x => x.UserKey == userId && x.BlockedUserKey == senderKey);
                            if (blocked != null) { DB.FriendBlocks.Remove(blocked); }
                        }
                        break;

                    case FriendStatus.None:
                        request = DB.FriendRequests.FirstOrDefault(x => (x.UserSenderKey == senderKey && x.UserReceiverKey == userId)
                                        || (x.UserSenderKey == userId && x.UserReceiverKey == senderKey));
                        if (request != null)
                        {
                            DB.FriendRequests.Remove(request);
                            var blocked = DB.FriendBlocks.FirstOrDefault(x => x.UserKey == userId && x.BlockedUserKey == senderKey);
                            if (blocked != null) { DB.FriendBlocks.Remove(blocked); }
                        }
                        break;
                }



                DB.SaveChanges();
            }
        }

        public List<NotificationViewModel> GetNotifications(Guid userId, bool unreadOnly = true)
        {
            var notifications = new List<NotificationViewModel>();

            using (DBReset())
            {
                notifications = DB.Notifications.Where(x => x.UserKey == userId && (!unreadOnly || !x.IsRead)).Select(x => new NotificationViewModel
                {
                    DateAdded = x.DateAdded,
                    IsRead = x.IsRead,
                    Link = x.Link,
                    Message = x.Message,
                    NotificationKey = x.NotificationKey,
                    UserKey = x.UserKey
                }).ToList();

                foreach (var item in notifications)
                {
                    var diff = DateTime.UtcNow - item.DateAdded;
                    if (diff.Days > 6) { item.Age = (diff.Days / 7).ToString() + "w"; }
                    else if (diff.Days > 0) { item.Age = diff.Days.ToString() + "d"; }
                    else if (diff.Hours > 0) { item.Age = diff.Hours.ToString() + "h"; }
                    else if (diff.Minutes > 0) { item.Age = diff.Minutes.ToString() + "m"; }
                    else { item.Age = diff.Seconds.ToString() + "s"; }
                }
            }

            return notifications;
        }

        public void AddNotification(string username, NotificationViewModel model)
        {
            using (DBReset())
            {
                var user = DB.AspNetUsers.FirstOrDefault(x => String.Compare(x.UserName, username, true) == 0);

                if (user != null)
                {
                    DB.Notifications.Add(new Notification
                    {
                        DateAdded = DateTime.UtcNow,
                        IsRead = false,
                        Link = model.Link,
                        Message = model.Message,
                        UserKey = user.UserId,
                        NotificationKey = model.NotificationKey
                    });
                }

                DB.SaveChanges();
            }
        }

        public void ReadNotification(Guid id)
        {
            using (DBReset())
            {
                var notification = DB.Notifications.FirstOrDefault(x => x.NotificationKey == id);

                if (notification != null)
                {
                    notification.IsRead = true;
                }

                DB.SaveChanges();
            }
        }
    }
}
