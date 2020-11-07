using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [Authorize]
    public class CharacterController : DeneirsController
    {
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult Index()
        {
            var model = new List<CharacterShortViewModel>();
            try
            {
                model = CharacterSvc.GetAllCharacters(AppUser.UserId, AppUser.ActiveCampaign.Value);

                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
            }
            catch (Exception ex) { }

            return View(model);
        }

        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public ActionResult CreateCharacter(bool isPlayer = false)
        {
            return RedirectToAction("EditCharacter", new { id = Guid.NewGuid(), isPlayer = isPlayer, isNew = true });
        }

        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.Player)]
        public ActionResult EditCharacter(Guid id, bool isPlayer = false, bool isNew = false)
        {
            var player = new PlayerViewModel();
            try
            {
                player = CharacterSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.IsPlayer = isPlayer;
                ViewBag.IsNew = isNew;
                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
            }
            catch (Exception ex) { }

            return View(player);
        }

        [HttpPost]
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.Player)]
        public JsonResult UpdatePlayer(PlayerPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CharacterSvc.UpdateCharacter(AppUser.UserId, AppUser.ActiveCampaign.Value, model, true, model.UserKey);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return Json(new { success = true, message = "Updated successfully!" });
            }
            return Json(new { success = false, message = GetValidationError() });
        }

        [HttpPost]
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult UpdateCharacter(CharacterPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CharacterSvc.UpdateCharacter(AppUser.UserId, AppUser.ActiveCampaign.Value, model, false);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return Json(new { success = true, message = "Updated successfully!" });
            }
            return Json(new { success = false, message = GetValidationError() });
        }

        [HttpPost]
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult DeleteCharacter(Guid id)
        {
            try
            {
                CharacterSvc.DeleteCharacter(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Deleted successfully!" });
        }
    }
}