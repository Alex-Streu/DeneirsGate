using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    public class DeneirsController : Controller
    {
        private UserDataModel userData;

        private AuthService authSvc;
        private CampaignService campaignSvc;

        public UserDataModel UserData
        {
            get
            {
                if (userData == null) { userData = AppLogic.GetUser(); }
                return userData;
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
    }
}