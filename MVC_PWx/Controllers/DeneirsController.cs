﻿using DeneirsGate.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private CharacterService characterSvc;
        private PresetService presetSvc;
        private UserService userSvc;
        private RelationshipTreeService relationshipTreeSvc;
        private MonsterService monsterSvc;
        private MagicItemService magicItemSvc;
        private EventService eventSvc;
        private DungeonService dungeonSvc;
        private SettlementService settlementSvc;
        private SuggestionService suggestionSvc;

        public Dictionary<string, DateTime> OnlineUsers
        {
            get { return (Dictionary<string, DateTime>)HttpContext.Application["OnlineUsers"]; }
        }

        public string CampaignName
        {
            get
            {
                var name = (string)HttpContext.Session["CampaignName"];
                if (name == null && AppUser.ActiveCampaign != null)
                {
                    name = CampaignSvc.GetCampaignName(AppUser.ActiveCampaign.GetValueOrDefault());
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

        public PresetService PresetSvc
        {
            get
            {
                if (presetSvc == null) { presetSvc = new PresetService(); }
                return presetSvc;
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

        public CharacterService CharacterSvc
        {
            get
            {
                if (characterSvc == null) { characterSvc = new CharacterService(); }
                return characterSvc;
            }
        }

        public UserService UserSvc
        {
            get
            {
                if (userSvc == null) { userSvc = new UserService(); }
                return userSvc;
            }
        }

        public RelationshipTreeService RelationshipTreeSvc
        {
            get
            {
                if (relationshipTreeSvc == null) { relationshipTreeSvc = new RelationshipTreeService(); }
                return relationshipTreeSvc;
            }
        }

        public MonsterService MonsterSvc
        {
            get
            {
                if (monsterSvc == null) { monsterSvc = new MonsterService(); }
                return monsterSvc;
            }
        }

        public MagicItemService MagicItemSvc
        {
            get
            {
                if (magicItemSvc == null) { magicItemSvc = new MagicItemService(); }
                return magicItemSvc;
            }
        }

        public EventService EventSvc
        {
            get
            {
                if (eventSvc == null) { eventSvc = new EventService(); }
                return eventSvc;
            }
        }

        public DungeonService DungeonSvc
        {
            get
            {
                if (dungeonSvc == null) { dungeonSvc = new DungeonService(); }
                return dungeonSvc;
            }
        }

        public SettlementService SettlementSvc
        {
            get
            {
                if (settlementSvc == null) { settlementSvc = new SettlementService(); }
                return settlementSvc;
            }
        }

        public SuggestionService SuggestionSvc
        {
            get
            {
                if (suggestionSvc == null) { suggestionSvc = new SuggestionService(); }
                return suggestionSvc;
            }
        }

        protected string GetValidationError()
        {
            return ModelState.Values.FirstOrDefault(x => x.Errors.Count > 0).Errors.FirstOrDefault().ErrorMessage;
        }

        protected JsonResult GetJson(bool success, string message = null, object data = null, ErrorPostModel error = null)
        {
            return Json(new { success = success, message = message, data = data, error = error });
        }

        protected JsonResult GetJson(ErrorPostModel error)
        {
            return GetJson(false, null, null, error);
        }

        protected ActionResult RedirectError(string error)
        {
            return RedirectToAction("Error500", "Errors", new { error = error });
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
            System.Web.HttpContext.Current.Session["ActiveCampaign"] = campaign;

            if (updateUser)
            {
                AppUser.ActiveCampaign = campaign;
                HttpContext.Session["AppUser"] = AppUser;

                var user = UserManager.FindByName(User.Identity.Name);
                user.ActiveCampaign = campaign;
                UserManager.Update(user);
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.User = AppUser;
                ViewBag.Notifications = UserSvc.GetNotifications(AppUser.UserId);
                ViewBag.Friends = UserSvc.GetFriends(AppUser.UserId, OnlineUsers, true);
                ViewBag.CampaignKey = AppUser.ActiveCampaign.GetValueOrDefault();
                ViewBag.CampaignName = CampaignName;
            }

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