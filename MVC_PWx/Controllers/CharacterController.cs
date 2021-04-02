using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

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
            catch (Exception ex)
            {
                return HandleExceptionRedirectError(ex);
            }

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
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Updated successfully!");
            }
            return HandleValidationJsonErrorResponse();
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
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Updated successfully!");
            }
            return HandleValidationJsonErrorResponse();
        }

        [HttpPost]
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult DeleteCharacter(Guid id)
        {
            try
            {
                CharacterSvc.DeleteCharacter(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                var path = AppLogic.GetCharacterContentDir(AppUser.ActiveCampaign.Value, id);
                var fullPath = Server.MapPath(path);
                var dirInfo = new DirectoryInfo(fullPath);
                dirInfo.Delete(true);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!");
        }

        [HttpPost]
        [HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
        public JsonResult SuggestCharacter()
        {
            var character = new CharacterViewModel();
            try
            {
                character = SuggestionSvc.GenerateCharacter();

                var rand = new Random();
                var alignments = PresetSvc.GetAlignments();
                var toSkip = rand.Next(0, alignments.Count);
                character.Alignment = alignments.OrderBy(x => Guid.NewGuid()).Skip(toSkip).Take(1).FirstOrDefault().Key;
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Deleted successfully!", character);
        }
    }
}