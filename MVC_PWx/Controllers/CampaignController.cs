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

        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult Dashboard(Guid id)
        {
            var model = new CampaignDashboardViewModel();
            try
            {
                model = CampaignSvc.GetCampaignDashboard(UserData.UserId, id);
            }
            catch (Exception ex) { }

            return View(model);
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult CreatePlayer(Guid id)
        {
            return RedirectToAction("EditPlayer", new { ownerId = id, id = Guid.NewGuid() });
        }

        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult EditPlayer(Guid ownerId, Guid id)
        {
            var player = new PlayerViewModel();
            try
            {
                player = CampaignSvc.GetPlayer(UserData.UserId, ownerId, id);
            }
            catch (Exception ex) { }

            return View(player);
        }

        [HttpPost]
        [HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult UpdatePlayer(PlayerPostModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CampaignSvc.UpdatePlayer(UserData.UserId, model);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Updated successfully!" });
        }
    }
}