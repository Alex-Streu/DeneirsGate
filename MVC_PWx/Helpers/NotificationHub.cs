using DeneirsGate.Services;
using Microsoft.AspNet.SignalR;
using System;

namespace DeneirsGateSite
{
    public class NotificationHub : Hub
    {
        private UserService userSvc;

        public NotificationHub(UserService userSvc)
        {
            this.userSvc = userSvc;
        }

        //public UserService UserSvc
        //{
        //    get
        //    {
        //        if (userSvc == null) { userSvc = new UserService(); }
        //        return userSvc;
        //    }
        //}

        public void Send(string username, NotificationViewModel notification)
        {
            if (!notification.Link.IsNullOrEmpty())
            {
                notification.Link = $"/Users/ReadNotification?id={notification.NotificationKey.ToString()}&returnUrl={notification.Link}";
            }
            userSvc.AddNotification(username, notification);
            Clients.User(username).addNotification(notification);
        }
    }

    public interface IUserIdProvider
    {
        string GetUserId(IRequest request);
    }
}