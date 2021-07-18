using DeneirsGate.Data;
using DeneirsGate.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    public class DeneirsController : Controller
    {
        private ApplicationUser appUser;
        private ApplicationUserManager userManager;
        private ApplicationSignInManager signInManager;
        private ApplicationRoleManager roleManager;
        private CampaignService campaignSvc;
        private UserService userSvc;

        private string _keyOnlineUsers = "OnlineUsers";
        private string _keyOfflineUsers = "OfflineUsers";

        public Dictionary<string, DateTime> OnlineUsers
        {
            get { return (Dictionary<string, DateTime>)HttpContext.Application[_keyOnlineUsers]; }
        }

        public string CampaignName
        {
            get
            {
                var name = (string)HttpContext.Session["CampaignName"];
                if (name == null && AppUser.ActiveCampaign != null)
                {
                    if (campaignSvc == null)
                    {
                        var db = new DataEntities();
                        campaignSvc = new CampaignService(db);
                    }
                    name = campaignSvc.GetCampaignName(AppUser.ActiveCampaign.GetValueOrDefault());
                    HttpContext.Session["CampaignName"] = name;
                }

                return name;
            }

            set
            {
                HttpContext.Session["CampaignName"] = value;
            }
        }

        public ApplicationUser AppUser
        {
            get
            {
                if (appUser == null) 
                {
                    appUser = (ApplicationUser)HttpContext.Session["AppUser"];
                    if (appUser == null)
                    {
                        appUser = UserManager.FindByName(User.Identity.Name);
                        HttpContext.Session["AppUser"] = appUser;
                    }
                }
                return appUser;
            }

            set
            {
                HttpContext.Session["AppUser"] = value;
            }
        }

        protected ApplicationUserManager UserManager
        {
            get
            {
                return userManager ?? HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
            set
            {
                userManager = value;
            }
        }

        protected ApplicationSignInManager SignInManager
        {
            get
            {
                return signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                signInManager = value;
            }
        }

        protected ApplicationRoleManager RoleManager
        {
            get
            {
                return roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            set
            {
                roleManager = value;
            }
        }

        public DeneirsController()
        {

        }

        public DeneirsController(ApplicationUserManager userManager, ApplicationRoleManager roleManager, CampaignService campaignService, UserService userService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            campaignSvc = campaignService;
            userSvc = userService;
        }

        protected string GetValidationError()
        {
            return ModelState.Values.FirstOrDefault(x => x.Errors.Count > 0).Errors.FirstOrDefault().ErrorMessage;
        }

        protected JsonResult GetJson(bool success, string message = null, object data = null, ErrorPostModel error = null)
        {
            return Json(new { success = success, message = message, data = data, error = error });
        }

        protected JsonResult GetJson(ErrorPostModel error, Exception ex = null)
        {
            if (ex != null)
            {
                SentrySdk.WithScope(scope =>
                {
                    scope.User = new User
                    {
                        Username = AppUser?.UserName
                    };
                    SentrySdk.CaptureException(ex);
                });
            }
            return GetJson(false, null, null, error);
        }

        protected ActionResult RedirectError(string error)
        {
            return RedirectToAction("Error500", "Errors", new { error = error });
        }

        protected ActionResult RedirectError(Exception ex)
        {
            return RedirectError(ex.InnerException?.Message ?? ex.Message);
        }

        protected ActionResult HandleExceptionRedirectError(Exception ex)
        {
            SentrySdk.WithScope(scope =>
            {
                scope.User = new User
                {
                    Username = AppUser?.UserName
                };
                SentrySdk.CaptureException(ex);
            });
            return RedirectError(ex);
        }

        protected PartialViewResult HandleExceptionRedirectErrorPartial(Exception ex)
        {
            SentrySdk.WithScope(scope =>
            {
                scope.User = new User
                {
                    Username = AppUser?.UserName
                };
                SentrySdk.CaptureException(ex);
            });
            return PartialView("ErrorPartial");
        }

        protected JsonResult HandleExceptionJsonErrorResponse(Exception ex, object data = null)
        {
            SentrySdk.WithScope(scope =>
            {
                scope.User = new User
                {
                    Username = AppUser?.UserName
                };
                SentrySdk.CaptureException(ex);
            });
            return GetJson(false, ex.InnerException?.Message ?? ex.Message, data);
        }

        protected JsonResult HandleValidationJsonErrorResponse(object data = null)
        {
            return GetJson(false, GetValidationError(), data);
        }

        protected void LogError(string error)
        {
            SentrySdk.WithScope(scope =>
            {
                scope.User = new User
                {
                    Username = AppUser?.UserName
                };
                SentrySdk.CaptureMessage(error);
            });
        }

        protected void LogException(Exception ex)
        {
            SentrySdk.WithScope(scope =>
            {
                scope.User = new User
                {
                    Username = AppUser?.UserName
                };
                SentrySdk.CaptureException(ex);
            });
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("/", "Campaign");
        }

        protected void SetActiveCampaign(Guid? campaign, bool updateUser = true)
        {
            HttpContext.Session["ActiveCampaign"] = campaign;

            if (updateUser)
            {
                AppUser.ActiveCampaign = campaign;
                HttpContext.Session["AppUser"] = AppUser;

                var user = UserManager.FindByName(User.Identity.Name);
                user.ActiveCampaign = campaign;
                UserManager.Update(user);
            }
        }

        protected void RemoveOnlineUser()
        {
            var offlineUsers = (List<string>)HttpContext.Application[_keyOfflineUsers] ?? new List<string>();
            offlineUsers.Add(User.Identity.Name);
            HttpContext.Application[_keyOfflineUsers] = offlineUsers;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (userSvc == null) 
                {
                    var db = new DataEntities();
                    userSvc = new UserService(db);
                }

                ViewBag.User = AppUser;
                ViewBag.Notifications = userSvc.GetNotifications(AppUser.UserId);
                ViewBag.Friends = userSvc.GetFriends(AppUser.UserId, OnlineUsers, true);
                ViewBag.CampaignKey = AppUser.ActiveCampaign.GetValueOrDefault();
                ViewBag.CampaignName = CampaignName;
            }

            base.OnActionExecuted(filterContext);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (userManager != null)
                {
                    UserManager.Dispose();
                    userManager = null;
                }

                if (roleManager != null)
                {
                    RoleManager.Dispose();
                    roleManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}