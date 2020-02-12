using System.Web.Mvc;
using static MVC_PWx.Helpers.AppLogic;

namespace MVC_PWx.Helpers
{
    public class AnonymousOnlyAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("/");
            }
        }
    }

    public class HasAccess : AuthorizeAttribute
    {
        public Priviledge Priviledge { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!HasPriviledge(Priviledge))
            {
                filterContext.Result = new RedirectResult("/");
            }
        }
    }
}