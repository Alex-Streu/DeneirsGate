﻿using DeneirsGate.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVC_PWx.Helpers;
using System.Web;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    public class DeneirsController : Controller
    {
        private ApplicationUser appUser;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        private AuthService authSvc;
        private CampaignService campaignSvc;

        public ApplicationUser AppUser
        {
            get
            {
                if (appUser == null) { appUser = UserManager.FindByName(User.Identity.Name); }
                return appUser;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            set
            {
                _roleManager = value;
            }
        }

        public AuthService AuthSvc
        {
            get
            {
                if (authSvc == null) { authSvc = new AuthService(); }
                return authSvc;
            }
        }

        public CampaignService CampaignSvc
        {
            get
            {
                if (campaignSvc == null) { campaignSvc = new CampaignService(); }
                return campaignSvc;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.User = AppUser;
            base.OnActionExecuted(filterContext);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}