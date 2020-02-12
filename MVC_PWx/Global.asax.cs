using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
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
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                var authSvc = new AuthService();
                var user = authSvc.GetUserData(User.Identity.Name);
                AppLogic.SetUser(user);
            }
        }
    }
}
