using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    public class CharacterController : DeneirsController
    {
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult Index()
        {
            var model = new List<CharacterShortViewModel>();
            try
            {
                model = CampaignSvc.GetAllCharacters(AppUser.UserId, AppUser.ActiveCampaign.Value);

                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
            }
            catch (Exception ex) { }

            return View(model);
        }

        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult CreateCharacter(bool isPlayer = false)
        {
            return RedirectToAction("EditCharacter", new { id = Guid.NewGuid(), isPlayer });
        }

        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.Player)]
        public ActionResult EditCharacter(Guid id, bool isPlayer = false)
        {
            var player = new PlayerViewModel();
            try
            {
                player = CampaignSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.IsPlayer = isPlayer;
            }
            catch (Exception ex) { }

            return View(player);
        }

        [HttpPost]
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.Player)]
        public JsonResult UpdatePlayer(PlayerPostModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CampaignSvc.UpdateCharacter(AppUser.UserId, model, true, model.UserKey);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Updated successfully!" });
        }

        [HttpPost]
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult UpdateCharacter(PlayerPostModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CampaignSvc.UpdateCharacter(AppUser.UserId, model, false, model.UserKey);
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