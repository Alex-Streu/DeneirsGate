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
        public UserService(DataEntities _db)
        {
            db = _db;
        }

        public FriendStatus CheckFriendStatus(Guid userId, Guid checkId)
        {
            var status = FriendStatus.None;
            var check = db.FriendRequests.FirstOrDefault(x => (x.UserSenderKey == userId && x.UserReceiverKey == checkId)
                                                                || (x.UserSenderKey == checkId && x.UserReceiverKey == userId))?.Status;
            status = (FriendStatus)(check ?? (int)FriendStatus.None);

            return status;
        }

        public List<FriendViewModel> GetFriends(Guid userId, Dictionary<string, DateTime> onlineUsers, bool onlineOnly = false)
        {
            if (onlineUsers == null) { onlineUsers = new Dictionary<string, DateTime>(); }
            var friends = new List<FriendViewModel>();

            var keys = db.FriendRequests.Where(x => x.UserSenderKey == userId && x.Status == (int)FriendStatus.Accepted).Select(x => x.UserReceiverKey).ToList();
            keys.AddRange(db.FriendRequests.Where(x => x.UserReceiverKey == userId && x.Status == (int)FriendStatus.Accepted).Select(x => x.UserSenderKey).ToList());

            friends = db.AspNetUsers.Where(x => keys.Contains(x.UserId)).Select(x => new FriendViewModel
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

            return friends;
        }

        public List<FriendViewModel> GetRequests(Guid userId)
        {
            var friends = new List<FriendViewModel>();

            var keys = db.FriendRequests.Where(x => x.UserReceiverKey == userId && x.Status == (int)FriendStatus.Pending).Select(x => x.UserSenderKey).ToList();
            friends = db.AspNetUsers.Where(x => keys.Contains(x.UserId)).Select(x => new FriendViewModel
            {
                Picture = x.Picture,
                UserId = x.UserId,
                Username = x.UserName,
                Status = FriendStatus.Pending,
                LastUpdated = db.FriendRequests.FirstOrDefault(y => y.UserReceiverKey == userId && y.UserSenderKey == x.UserId).LastUpdated
            }).OrderBy(x => x.LastUpdated).ToList();

            return friends;
        }

        public List<FriendViewModel> GetPending(Guid userId)
        {
            var friends = new List<FriendViewModel>();

            var keys = db.FriendRequests.Where(x => x.UserSenderKey == userId && x.Status == (int)FriendStatus.Pending).Select(x => x.UserReceiverKey).ToList();
            friends = db.AspNetUsers.Where(x => keys.Contains(x.UserId)).Select(x => new FriendViewModel
            {
                Picture = x.Picture,
                UserId = x.UserId,
                Username = x.UserName,
                Status = FriendStatus.Pending,
                LastUpdated = db.FriendRequests.FirstOrDefault(y => y.UserSenderKey == userId && y.UserReceiverKey == x.UserId).LastUpdated
            }).OrderBy(x => x.LastUpdated).ToList();

            return friends;
        }

        public List<FriendViewModel> GetBlocked(Guid userId)
        {
            var friends = new List<FriendViewModel>();

            var keys = db.FriendBlocks.Where(x => x.UserKey == userId).Select(x => x.BlockedUserKey).ToList();
            friends = db.AspNetUsers.Where(x => keys.Contains(x.UserId)).Select(x => new FriendViewModel
            {
                Picture = x.Picture,
                UserId = x.UserId,
                Username = x.UserName,
                IsOnline = false,
                Status = FriendStatus.Blocked
            }).OrderBy(x => x.Username).ToList();

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
            var request = db.FriendRequests.FirstOrDefault(x => (x.UserSenderKey == userId && x.UserReceiverKey == model.RequestUserId)
                                                            || (x.UserSenderKey == model.RequestUserId && x.UserReceiverKey == userId));
            if (request == null)
            {
                db.FriendRequests.Add(new FriendRequest
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

            db.SaveChanges();
        }

        public void UpdateFriendRequest(Guid userId, Guid senderKey, FriendStatus status)
        {
            FriendRequest request = null;

            switch (status)
            {
                case FriendStatus.Accepted:
                case FriendStatus.Blocked:
                    request = db.FriendRequests.FirstOrDefault(x => (x.UserSenderKey == senderKey && x.UserReceiverKey == userId)
                                                                    || (x.UserSenderKey == userId && x.UserReceiverKey == senderKey));
                    if (request != null)
                    {
                        request.Status = (int)status;
                        request.LastUpdated = DateTime.UtcNow;
                    }
                    if (status == FriendStatus.Blocked)
                    {
                        db.FriendBlocks.Add(new FriendBlock
                        {
                            UserKey = userId,
                            BlockedUserKey = senderKey
                        });
                    }
                    else
                    {
                        var blocked = db.FriendBlocks.FirstOrDefault(x => x.UserKey == userId && x.BlockedUserKey == senderKey);
                        if (blocked != null) { db.FriendBlocks.Remove(blocked); }
                    }
                    break;

                case FriendStatus.None:
                    request = db.FriendRequests.FirstOrDefault(x => (x.UserSenderKey == senderKey && x.UserReceiverKey == userId)
                                    || (x.UserSenderKey == userId && x.UserReceiverKey == senderKey));
                    if (request != null)
                    {
                        db.FriendRequests.Remove(request);
                        var blocked = db.FriendBlocks.FirstOrDefault(x => x.UserKey == userId && x.BlockedUserKey == senderKey);
                        if (blocked != null) { db.FriendBlocks.Remove(blocked); }
                    }
                    break;
            }

            db.SaveChanges();
        }

        public List<NotificationViewModel> GetNotifications(Guid userId, bool unreadOnly = true)
        {
            var notifications = db.Notifications.Where(x => x.UserKey == userId && (!unreadOnly || !x.IsRead)).Select(x => new NotificationViewModel
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

            return notifications;
        }

        public void AddNotification(string username, NotificationViewModel model)
        {
            var user = db.AspNetUsers.FirstOrDefault(x => String.Compare(x.UserName, username, true) == 0);

            if (user != null)
            {
                db.Notifications.Add(new Notification
                {
                    DateAdded = DateTime.UtcNow,
                    IsRead = false,
                    Link = model.Link,
                    Message = model.Message,
                    UserKey = user.UserId,
                    NotificationKey = model.NotificationKey
                });
            }

            db.SaveChanges();
        }

        public void ReadNotification(Guid id)
        {
            var notification = db.Notifications.FirstOrDefault(x => x.NotificationKey == id);

            if (notification != null)
            {
                notification.IsRead = true;
            }

            db.SaveChanges();
        }

        public  void ReadNotifications(Guid userId)
        {
            var notifications = db.Notifications.Where(x => x.UserKey == userId && !x.IsRead).Select(x => x.NotificationKey).ToList();
            foreach (var notification in notifications)
            {
                ReadNotification(notification);
            }
        }

        public void DeleteNotification(Guid id)
        {
            db.Notifications.RemoveRange(x => x.NotificationKey == id);

            db.SaveChanges();
        }
    }
}
