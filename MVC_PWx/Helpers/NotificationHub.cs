using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DeneirsGate.Services;
using Microsoft.AspNet.SignalR;

namespace MVC_PWx
{
    public class NotificationHub : Hub
    {
        public void Send(string userId, NotificationModel notification)
        {
            Clients.User(userId).addNotification(notification);
        }
    }

    public interface IUserIdProvider
    {
        string GetUserId(IRequest request);
    }
}