using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVC_PWx
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if (Application["OnlineUsers"] == null) { Application["OnlineUsers"] = new Dictionary<string, DateTime>(); }
        }

        protected void Application_EndRequest()
        {
            //Get list
            var onlineUsers = (Dictionary<string, DateTime>)Application["OnlineUsers"];
            if (onlineUsers == null) { return; }

            //Update current user
            if (User.Identity.IsAuthenticated)
            {
                if (!onlineUsers.ContainsKey(User.Identity.Name)) { onlineUsers.Add(User.Identity.Name, DateTime.UtcNow); }
                else { onlineUsers[User.Identity.Name] = DateTime.UtcNow; }
            }

            //Remove inactive users and logged out user
            foreach (var s in onlineUsers.Where(x => x.Value <= DateTime.UtcNow.AddMinutes(-1)).ToList())
            {
                onlineUsers.Remove(s.Key);
            }

            //Set list
            Application["OnlineUsers"] = onlineUsers;
        }
    }
}
