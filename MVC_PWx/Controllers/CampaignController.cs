using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [Authorize]
    [HasAccess(Priviledge = AppLogic.Priviledge.Player)]
    public class CampaignController : DeneirsController
    {
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult Index()
        {
            var model = new CampaignDashboardViewModel();
            try
            {
                model = CampaignSvc.GetCampaignDashboard(AppUser.UserId, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex) { }

            return View(model);
        }

        public ActionResult ChangeCampaign()
        {
            var campaigns = new List<CampaignViewModel>();
            try
            {
                campaigns = CampaignSvc.GetCampaigns(AppUser.UserId, true);
            }
            catch (Exception ex) { }

            return View(campaigns);
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult ActivateCampaign(Guid? id)
        {
            if (id != null)
            {
                SetActiveCampaign(id);
            }

            return RedirectToAction("/");
        }
    }
}