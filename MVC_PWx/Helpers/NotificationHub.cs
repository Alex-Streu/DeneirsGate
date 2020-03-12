using DeneirsGate.Services;
using Microsoft.AspNet.SignalR;

namespace MVC_PWx
{
    public class NotificationHub : Hub
    {
        public void Send(string username, NotificationViewModel notification)
        {
            Clients.User(username).addNotification(notification);
        }
    }

    public interface IUserIdProvider
    {
        string GetUserId(IRequest request);
    }
}