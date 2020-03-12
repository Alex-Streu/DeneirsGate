using DeneirsGate.Data;
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

        public void AddNotification(NotificationViewModel model)
        {
            using (DBReset())
            {
                DB.Notifications.Add(new Notification
                {
                    DateAdded = DateTime.UtcNow,
                    IsRead = false,
                    Link = model.Link,
                    Message = model.Message,
                    UserKey = model.UserKey
                });

                DB.SaveChanges();
            }
        }
    }
}
