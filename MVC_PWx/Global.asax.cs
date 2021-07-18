using DeneirsGate.Services;
using Sentry;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DeneirsGateSite
{
    public class MvcApplication : HttpApplication
    {
        private IDisposable _sentry;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //DI
            UnityConfig.RegisterComponents();
            DeneirsGate.Services.UnityConfig.RegisterComponents();
            CustomHtmlHelpers.HtmlHelpers.Init();

            //Online Users
            if (Application["OnlineUsers"] == null) { Application["OnlineUsers"] = new Dictionary<string, DateTime>(); }

            //Sentry Logging
            _sentry = SentrySdk.Init(o =>
            {
                o.AddEntityFramework();
                o.Dsn = ConfigurationManager.AppSettings["sentryDSN"].ToString();
                o.Environment = ConfigurationManager.AppSettings["environment"].ToString();
            });
        }

        //protected void Application_BeginRequest(UserService userSvc)
        //{
        //    var appUser = (ApplicationUser)Session["AppUser"];
        //    var onlineUsers = (Dictionary<string, DateTime>)Application["OnlineUsers"];
        //    Session["Notifications"] = userSvc.GetNotifications(appUser.UserId);
        //    Session["Friends"] = userSvc.GetFriends(appUser.UserId, onlineUsers, true);
        //}

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            if (User.Identity.IsAuthenticated)
            {
                SentrySdk.WithScope(scope =>
                {
                    scope.User = new User
                    {
                        Username = User.Identity.Name
                    };
                    SentrySdk.CaptureException(exception);
                });
            }
            else
            {
                SentrySdk.CaptureException(exception);
            }
        }

        protected void Application_EndRequest()
        {
            //Get list
            var onlineUsers = (Dictionary<string, DateTime>)Application["OnlineUsers"];
            if (onlineUsers == null) { return; }

            var offlineUsers = (List<string>)Application["OfflineUsers"] ?? new List<string>();

            //Update current user
            if (User.Identity.IsAuthenticated)
            {
                if (!onlineUsers.ContainsKey(User.Identity.Name)) { onlineUsers.Add(User.Identity.Name, DateTime.UtcNow); }
                else { onlineUsers[User.Identity.Name] = DateTime.UtcNow; }
            }

            //Remove inactive users and logged out user
            foreach (var s in onlineUsers.Where(x => x.Value <= DateTime.UtcNow.AddMinutes(-1) || offlineUsers.Contains(x.Key)).ToList())
            {
                onlineUsers.Remove(s.Key);
            }

            //Set list
            Application["OnlineUsers"] = onlineUsers;
            Application["OfflineUsers"] = new List<string>();
        }

        protected void Application_End()
        {
            _sentry?.Dispose();
        }
    }
}
