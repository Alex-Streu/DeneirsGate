using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [HasAccess(Priviledge = AppLogic.Priviledge.Player)]
    public class CampaignController : DeneirsController
    {
        public ActionResult Index()
        {
            var campaigns = new List<CampaignViewModel>();
            try
            {
                campaigns = CampaignSvc.GetCampaigns(UserData.UserId, UserData.Priviledge > (int)AppLogic.Priviledge.Player);
            }
            catch (Exception ex) { }

            return View(campaigns);
        }

        public ActionResult Dashboard(Guid id)
        {
            try
            {
                CampaignSvc.GetCampaignDashboard(UserData.UserId, id);
            }
            catch (Exception ex) { }

            return View();
        }
    }
}