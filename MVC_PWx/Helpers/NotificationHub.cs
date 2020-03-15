using DeneirsGate.Services;
using Microsoft.AspNet.SignalR;
using System;

namespace MVC_PWx
{
    public class NotificationHub : Hub
    {
        private UserService userSvc;

        public UserService UserSvc
        {
            get
            {
                if (userSvc == null) { userSvc = new UserService(); }
                return userSvc;
            }
        }

        public void Send(string username, NotificationViewModel notification)
        {
            if (!notification.Link.IsNullOrEmpty())
            {
                notification.Link = $"/Users/ReadNotification?id={notification.NotificationKey.ToString()}&returnUrl={notification.Link}";
            }
            UserSvc.AddNotification(username, notification);
            Clients.User(username).addNotification(notification);
        }
    }

    public interface IUserIdProvider
    {
        string GetUserId(IRequest request);
    }
}